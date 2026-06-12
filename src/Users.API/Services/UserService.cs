using Users.API.DTOS;
using Users.API.Exceptions;
using Users.API.Models;
using Microsoft.Data.Sqlite;
using Dapper;

namespace Users.API.Services
{
    public class UserService : IUserService
    {
        private readonly string _connectionString;

        public UserService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection") ?? "Data Source=app.db";
        }

        private SqliteConnection CreateConnection() => new SqliteConnection(_connectionString);

        public async Task<UserResponseDTO> RegisterAsync(RegisterRequestDTO request)
        {
            using var conn = CreateConnection();

            var exists = await conn.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM users WHERE Email = @Email", new { request.Email });
            if (exists > 0)
            {
                throw new BusinessRuleException("USR-001", "El email ya esta registrado.");
            }

            string hashedPassword = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(request.Password));

            var newUser = new Users.API.Models.Users
            {
                Id = Guid.NewGuid(),
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                Email = request.Email,
                PasswordHash = hashedPassword,
                FechaRegistro = DateTime.UtcNow,
                Activo = true,
                IntentosFallidos = 0
            };

            await conn.ExecuteAsync("""
                INSERT INTO users (Id, Nombre, Apellido, Email, PasswordHash, FechaRegistro, Activo, IntentosFallidos)
                VALUES (@Id, @Nombre, @Apellido, @Email, @PasswordHash, @FechaRegistro, @Activo, @IntentosFallidos)
            """, new
            {
                Id = newUser.Id.ToString(),
                newUser.Nombre,
                newUser.Apellido,
                newUser.Email,
                newUser.PasswordHash,
                FechaRegistro = newUser.FechaRegistro.ToString("o"),
                Activo = newUser.Activo ? 1 : 0,
                newUser.IntentosFallidos
            });

            return new UserResponseDTO
            {
                Id = newUser.Id,
                Nombre = newUser.Nombre,
                Apellido = newUser.Apellido,
                Email = newUser.Email,
                FechaRegistro = newUser.FechaRegistro,
                Activo = newUser.Activo
            };
        }

        public async Task<UserResponseDTO> LoginAsync(LoginRequestDTO request)
        {
            using var conn = CreateConnection();

            var userRaw = await conn.QueryFirstOrDefaultAsync("""
                SELECT Id, Nombre, Apellido, Email, PasswordHash, FechaRegistro, Activo, IntentosFallidos
                FROM users WHERE Email = @Email
            """, new { request.Email });

            if (userRaw == null)
            {
                throw new BusinessRuleException("USR-003", "Credenciales incorrectas.");
            }

            var user = new Users.API.Models.Users
            {
                Id = Guid.Parse((string)userRaw.Id),
                Nombre = (string)userRaw.Nombre,
                Apellido = (string)userRaw.Apellido,
                Email = (string)userRaw.Email,
                PasswordHash = (string)userRaw.PasswordHash,
                FechaRegistro = DateTime.Parse((string)userRaw.FechaRegistro),
                Activo = Convert.ToBoolean(userRaw.Activo),
                IntentosFallidos = Convert.ToInt32(userRaw.IntentosFallidos)
            };

            if (!user.Activo && user.IntentosFallidos >= 3)
            {
                throw new BusinessRuleException("USR-004", "Usuario bloqueado por demasiados intentos fallidos.");
            }
            else if (!user.Activo)
            {
                throw new BusinessRuleException("USR-005", "Usuario bloqueado por detección de fraude.");
            }

            string hashedInputPassword = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(request.Password));

            if (user.PasswordHash != hashedInputPassword)
            {
                user.IntentosFallidos++;

                if (user.IntentosFallidos >= 3)
                {
                    user.Activo = false;
                    await conn.ExecuteAsync("UPDATE users SET IntentosFallidos = @IntentosFallidos, Activo = 0 WHERE Id = @Id", new { IntentosFallidos = user.IntentosFallidos, Id = user.Id.ToString() });
                    throw new BusinessRuleException("USR-004", "Su cuenta fue bloqueada por superar el máximo de intentos fallidos. Contacte a soporte.");
                }

                await conn.ExecuteAsync("UPDATE users SET IntentosFallidos = @IntentosFallidos WHERE Id = @Id", new { IntentosFallidos = user.IntentosFallidos, Id = user.Id.ToString() });
                throw new BusinessRuleException("USR-003", "Credenciales incorrectas.");
            }

            if (user.IntentosFallidos > 0)
            {
                await conn.ExecuteAsync("UPDATE users SET IntentosFallidos = 0 WHERE Id = @Id", new { Id = user.Id.ToString() });
            }

            return new UserResponseDTO
            {
                Id = user.Id,
                Nombre = user.Nombre,
                Apellido = user.Apellido,
                Email = user.Email,
                FechaRegistro = user.FechaRegistro,
                Activo = user.Activo
            };
        }

        public async Task<UserResponseDTO?> GetByIdAsync(Guid id)
        {
            using var conn = CreateConnection();

            var userRaw = await conn.QueryFirstOrDefaultAsync("""
                SELECT Id, Nombre, Apellido, Email, FechaRegistro, Activo 
                FROM users WHERE Id = @Id
            """, new { Id = id.ToString() });

            if (userRaw == null)
            {
                return null;
            }

            return new UserResponseDTO
            {
                Id = Guid.Parse((string)userRaw.Id),
                Nombre = (string)userRaw.Nombre,
                Apellido = (string)userRaw.Apellido,
                Email = (string)userRaw.Email,
                FechaRegistro = DateTime.Parse((string)userRaw.FechaRegistro),
                Activo = Convert.ToBoolean(userRaw.Activo)
            };
        }
    }
}
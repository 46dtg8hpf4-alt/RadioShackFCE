using Users.API.DTOS;
using Users.API.Exceptions;
using Users.API.Models;

namespace Users.API.Services
{
    public class UserService : IUserService
    { 
        private static readonly List<Users.API.Models.Users> _usersDb = new List<Users.API.Models.Users>();

        public async Task<UserResponseDTO> RegisterAsync(RegisterRequestDTO request)
        {
            var exists = _usersDb.Any(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));
            if (exists)
            {
                throw new BusinessRuleException("USR-001", "El correo electrónico ya se encuentra registrado.");
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

            _usersDb.Add(newUser);

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
            var user = _usersDb.FirstOrDefault(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                throw new BusinessRuleException("USR-003", "Credenciales incorrectas."); 
            }

            if (!user.Activo && user.IntentosFallidos >= 3)
            {
                throw new BusinessRuleException("USR-004", "Su cuenta fue bloqueada por superar el máximo de intentos fallidos. Contacte a soporte."); 
            }
            else if (!user.Activo)
            {
                throw new BusinessRuleException("USR-005", "Su cuenta fue suspendida por razones de seguridad. Contacte a soporte."); 
            }

            string hashedInputPassword = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(request.Password));

            if (user.PasswordHash != hashedInputPassword)
            {
                user.IntentosFallidos++; 

                if (user.IntentosFallidos >= 3)
                {
                    user.Activo = false;
                    throw new BusinessRuleException("USR-004", "Su cuenta fue bloqueada por superar el máximo de intentos fallidos. Contacte a soporte."); 
                }

                throw new BusinessRuleException("USR-003", "Credenciales incorrectas."); 
            }

            user.IntentosFallidos = 0; 

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
            var user = _usersDb.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return null;
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
    }
}
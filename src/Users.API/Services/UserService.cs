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
            return new UserResponseDTO();
        }
    }
}
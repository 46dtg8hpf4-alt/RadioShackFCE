using Users.API.DTOS;

namespace Users.API.Services
{
    public interface IUserService
    {
        Task<UserResponseDTO> RegisterAsync(RegisterRequestDTO request);

        Task<UserResponseDTO> LoginAsync(LoginRequestDTO request);

        Task<UserResponseDTO?> GetByIdAsync(Guid id);
    }
}
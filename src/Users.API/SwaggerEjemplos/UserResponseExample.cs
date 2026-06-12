using Swashbuckle.AspNetCore.Filters;
using Users.API.DTOS;

namespace Users.API.SwaggerExamples
{
    public class UserResponseExample : IExamplesProvider<UserResponseDTO>
    {
        public UserResponseDTO GetExamples()
        {
            return new UserResponseDTO
            {
                Id = Guid.Parse("a1b2c3d4-0000-0000-0000-111122223333"),
                Nombre = "Ivan",
                Apellido = "Mammes",
                Email = "Ivan@email.com",
                FechaRegistro = DateTime.Parse("2024-03-10T09:00:00Z"),
                Activo = true
            };
        }
    }
}

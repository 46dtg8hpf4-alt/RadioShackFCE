using Swashbuckle.AspNetCore.Filters;
using Users.API.DTOS;

namespace Users.API.SwaggerExamples
{
    public class RegisterRequestExample : IExamplesProvider<RegisterRequestDTO>
    {
        public RegisterRequestDTO GetExamples()
        {
            return new RegisterRequestDTO
            {
                Nombre = "Ivan",
                Apellido = "Mammes",
                Email = "ivan@email.com",
                Password = "Independiente123!"
            };
        }
    }
}

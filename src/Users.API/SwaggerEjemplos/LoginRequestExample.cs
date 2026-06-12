using Swashbuckle.AspNetCore.Filters;
using Users.API.DTOS;

namespace Users.API.SwaggerExamples
{
    public class LoginRequestExample : IExamplesProvider<LoginRequestDTO>
    {
        public LoginRequestDTO GetExamples()
        {
            return new LoginRequestDTO
            {
                Email = "ivan@email.com",
                Password = "Independiente123!"
            };
        }
    }
}

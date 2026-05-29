using Microsoft.AspNetCore.Mvc;
using Users.API.DTOS;
using Users.API.Services;

namespace Users.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            var response = await _userService.RegisterAsync(request);

            return CreatedAtAction(nameof(Register), new { id = response.Id }, response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            var response = await _userService.LoginAsync(request);

            return Ok(response);
        }
    }
}

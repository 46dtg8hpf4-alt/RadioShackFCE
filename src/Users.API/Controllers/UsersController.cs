using Microsoft.AspNetCore.Mvc;
using Users.API.DTOS;
using Users.API.Services;

namespace Users.API.Controllers
{
    /// <summary>
    /// API para la gestión y autenticación de Usuarios
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Registrar nuevo usuario
        /// </summary>
        /// <param name="request">Datos requeridos para el registro del usuario.</param>
        /// <returns>El usuario recién registrado.</returns>
        /// <response code="201">Usuario creado exitosamente.</response>
        /// <response code="400">Los datos del usuario son inválidos.</response>
        /// <response code="409">El email ya está registrado.</response>
        /// <response code="500">Error interno al procesar el usuario.</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(UserResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            var response = await _userService.RegisterAsync(request);

            return CreatedAtAction(nameof(Register), new { id = response.Id }, response);
        }

        /// <summary>
        /// Autenticar usuario
        /// </summary>
        /// <param name="request">Credenciales del usuario (email y password).</param>
        /// <returns>Los datos del usuario autenticado.</returns>
        /// <response code="200">Autenticación exitosa.</response>
        /// <response code="400">Los datos del usuario son inválidos.</response>
        /// <response code="401">Credenciales incorrectas.</response>
        /// <response code="403">Usuario bloqueado por intentos fallidos o detección de fraude.</response>
        /// <response code="500">Error interno al procesar el usuario.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(UserResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            var response = await _userService.LoginAsync(request);

            return Ok(response);
        }

        /// <summary>
        /// Obtener usuario por ID
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>El usuario solicitado.</returns>
        /// <response code="200">Usuario encontrado.</response>
        /// <response code="404">Usuario no encontrado.</response>
        /// <response code="500">Error interno al procesar el usuario.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
    }
}


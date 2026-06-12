using System.ComponentModel.DataAnnotations;

namespace Users.API.DTOS
{
    /// <summary>
    /// Datos de solicitud para el login de un usuario
    /// </summary>
    public class LoginRequestDTO
    {
        /// <summary>
        /// Dirección de email
        /// </summary>
        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Contraseña del usuario
        /// </summary>
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Password { get; set; } = string.Empty;
    }
}

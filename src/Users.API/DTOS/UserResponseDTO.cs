namespace Users.API.DTOS
{
    /// <summary>
    /// Respuesta con los datos públicos del usuario
    /// </summary>
    public class UserResponseDTO
    {
        /// <summary>
        /// Identificador usuario
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre del usuario
        /// </summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Apellido del usuario
        /// </summary>
        public string Apellido { get; set; } = string.Empty;

        /// <summary>
        /// Dirección de correo electrónico
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Fecha y hora en la que el usuario se registró
        /// </summary>
        public DateTime FechaRegistro { get; set; }

        /// <summary>
        /// Indica si el usuario está activo o bloqueado
        /// </summary>
        public bool Activo { get; set; }
    }
}
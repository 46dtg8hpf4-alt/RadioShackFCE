namespace Users.API.Models
{
    public class Users
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        // Contraseña encriptada
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public bool Activo { get; set; } = true;

        public int IntentosFallidos { get; set; } = 0;
    }
}

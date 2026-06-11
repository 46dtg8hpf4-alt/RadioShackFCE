namespace Notifications.API.DTOs
{
    public class SendNotificationRequest
    {
        public Guid UsuarioId { get; set; }

        public string Mensaje { get; set; }

        public string Tipo { get; set; }

        public SendNotificationRequest()
        {
            Mensaje = "";
            Tipo = "";
        }
    }
}
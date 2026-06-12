using Microsoft.AspNetCore.Mvc;
using Notifications.API.DTOs;
using Notifications.API.Models;
using Notifications.API.Services;

namespace Notifications.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationService _service;

        public NotificationsController(
            NotificationService service)
        {
            _service = service;
        }

        [HttpPost("send")]
        public async Task<ActionResult<NotificationResponse>>
            SendNotification(
                SendNotificationRequest request)
        {
            Notification notification =
                await _service.SendNotificationAsync(
                    request);

            NotificationResponse response =
                new NotificationResponse
                {
                    Id = notification.Id,
                    UsuarioId = notification.UsuarioId,
                    Mensaje = notification.Mensaje,
                    Tipo = notification.Tipo,
                    Estado = notification.Estado,
                    FechaEnvio = notification.FechaEnvio
                };

            return Created("", response);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<
            List<NotificationResponse>>>
            GetNotifications(Guid userId)
        {
            List<Notification> notifications =
                await _service.GetNotificationsByUserAsync(
                    userId);

            List<NotificationResponse> response =
                new();

            foreach (var notification in notifications)
            {
                response.Add(
                    new NotificationResponse
                    {
                        Id = notification.Id,
                        UsuarioId = notification.UsuarioId,
                        Mensaje = notification.Mensaje,
                        Tipo = notification.Tipo,
                        Estado = notification.Estado,
                        FechaEnvio = notification.FechaEnvio
                    });
            }

            return Ok(response);
        }
    }
}
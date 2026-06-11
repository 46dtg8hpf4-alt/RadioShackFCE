using Notifications.API.Clients;
using Notifications.API.Data;
using Notifications.API.DTOs;
using Notifications.API.Exceptions;
using Notifications.API.Models;

namespace Notifications.API.Services
{
    public class NotificationService
    {
        private readonly NotificationRepository _repository;
        private readonly UsersApiClient _usersApiClient;

        public NotificationService(
            NotificationRepository repository,
            UsersApiClient usersApiClient)
        {
            _repository = repository;
            _usersApiClient = usersApiClient;
        }

        public async Task<Notification> SendNotificationAsync(
            SendNotificationRequest request)
        {
            bool userExists =
                await _usersApiClient.UserExists(
                    request.UsuarioId);

            if (!userExists)
            {
                throw new BusinessRuleException(
                    "NTF-001",
                    "Usuario no encontrado.");
            }

            if (string.IsNullOrWhiteSpace(request.Mensaje))
            {
                throw new BusinessRuleException(
                    "NTF-002",
                    "Datos inválidos.");
            }

            if (
                request.Tipo != "Email" &&
                request.Tipo != "SMS" &&
                request.Tipo != "Push")
            {
                throw new BusinessRuleException(
                    "NTF-002",
                    "Datos inválidos.");
            }

            Notification notification =
                new Notification
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = request.UsuarioId,
                    Mensaje = request.Mensaje,
                    Tipo = request.Tipo,
                    Estado = "Enviada",
                    FechaEnvio = DateTime.UtcNow
                };

            await _repository.CreateAsync(notification);

            return notification;
        }

        public async Task<List<Notification>>
            GetNotificationsByUserAsync(Guid userId)
        {
            List<Notification> notifications =
                await _repository.GetByUserIdAsync(userId);

            if (notifications.Count == 0)
            {
                throw new BusinessRuleException(
                    "NTF-003",
                    "No se encontraron notificaciones.");
            }

            return notifications;
        }
    }
}
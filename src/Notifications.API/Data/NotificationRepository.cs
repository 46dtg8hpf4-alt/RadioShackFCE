using Dapper;
using Microsoft.Data.Sqlite;
using Notifications.API.Models;

namespace Notifications.API.Data
{
    public class NotificationRepository
    {
        private readonly IConfiguration _config;

        public NotificationRepository(IConfiguration config)
        {
            _config = config;
        }

        private SqliteConnection CreateConnection()
        {
            return new SqliteConnection(
                _config.GetConnectionString("DefaultConnection")
                ?? "Data Source=notifications.db");
        }

public async Task<List<Notification>> GetByUserIdAsync(Guid userId)
{
    using var conn = CreateConnection();

    var rows = await conn.QueryAsync(
        """
        SELECT *
        FROM Notifications
        WHERE UsuarioId = @UsuarioId
        ORDER BY FechaEnvio DESC
        """,
        new
        {
            UsuarioId = userId.ToString()
        });

    List<Notification> notifications = new();

    foreach (var row in rows)
    {
        notifications.Add(
            new Notification
            {
                Id = Guid.Parse(row.Id),
                UsuarioId = Guid.Parse(row.UsuarioId),
                Mensaje = row.Mensaje,
                Tipo = row.Tipo,
                Estado = row.Estado,
                FechaEnvio = DateTime.Parse(row.FechaEnvio)
            });
    }

    return notifications;
}

        public async Task CreateAsync(Notification notification)
        {
            using var conn = CreateConnection();

            await conn.ExecuteAsync(
                """
                INSERT INTO Notifications
                (
                    Id,
                    UsuarioId,
                    Mensaje,
                    Tipo,
                    Estado,
                    FechaEnvio
                )
                VALUES
                (
                    @Id,
                    @UsuarioId,
                    @Mensaje,
                    @Tipo,
                    @Estado,
                    @FechaEnvio
                )
                """,
                new
                {
                    Id = notification.Id.ToString(),
                    UsuarioId = notification.UsuarioId.ToString(),
                    notification.Mensaje,
                    notification.Tipo,
                    notification.Estado,
                    FechaEnvio = notification.FechaEnvio.ToString("o")
                });
        }
    }
}
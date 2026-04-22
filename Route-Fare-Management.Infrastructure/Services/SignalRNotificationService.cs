using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Route_Fare_Management.Application.Interfaces;

namespace Route_Fare_Management.Infrastructure.Services
{
    /// <summary>
    /// Sends export progress to connected SignalR clients in real time
    /// Delegates to IHubClientsAdapter interface so that Infrastructure never references
    /// the API project or the SignalR IClientProxy type directly.
    /// </summary>
    public class SignalRNotificationService : INotificationService
    {
        private readonly IHubClientsAdapter _adapter;
        private readonly ILogger<SignalRNotificationService> _logger;

        public SignalRNotificationService(
            IHubClientsAdapter adapter,
            ILogger<SignalRNotificationService> logger)
        {
            _adapter = adapter;
            _logger = logger;
        }

        public async Task SendProgressAsync(
            string connectionId, int progress, string message,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug(
                "Export {Progress}% → [{ConnectionId}]: {Message}",
                progress, connectionId, message);

            await _adapter.SendAsync(
                connectionId,
                "ProgressUpdate",
                new object?[] { progress, message },
                cancellationToken);
        }

        public async Task SendCompletedAsync(
            string connectionId, string fileUrl,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Export complete → [{ConnectionId}]: {Url}", connectionId, fileUrl);

            await _adapter.SendAsync(
                connectionId,
                "ExportComplete",
                new object?[] { fileUrl },
                cancellationToken);
        }

        public async Task SendErrorAsync(
            string connectionId, string error,
            CancellationToken cancellationToken = default)
        {
            _logger.LogWarning(
                "Export error → [{ConnectionId}]: {Error}", connectionId, error);

            await _adapter.SendAsync(
                connectionId,
                "ExportError",
                new object?[] { error },
                cancellationToken);
        }
    }

}

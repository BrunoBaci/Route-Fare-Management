using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Route_Fare_Management.API.Services
{
    /// <summary>
    /// signalR hub that streams Excel export progress
    /// </summary>
    // [Authorize]
    public class ExportProgressHub : Hub
    {
        private readonly ILogger<ExportProgressHub> _logger;

        public ExportProgressHub(ILogger<ExportProgressHub> logger)
            => _logger = logger;
        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation(
                "SignalR connected: ConnectionId={ConnectionId} User={User}",
                Context.ConnectionId,
                Context.UserIdentifier);

            await Clients.Caller.SendAsync("ReceiveConnectionId", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public async Task JoinExportJob(string jobId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, jobId);

            _logger.LogInformation(
                "Client joined export job {JobId} with connection {ConnectionId}",
                jobId, Context.ConnectionId);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (exception is not null)
                _logger.LogWarning(exception,
                    "SignalR disconnected with error: ConnectionId={ConnectionId}",
                    Context.ConnectionId);
            else
                _logger.LogInformation(
                    "SignalR disconnected: ConnectionId={ConnectionId}",
                    Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Route_Fare_Management.Application.Interfaces;

namespace Route_Fare_Management.API.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class HubClientsAdapter : IHubClientsAdapter
    {
        private readonly IHubContext<ExportProgressHub> _hub;

        public HubClientsAdapter(IHubContext<ExportProgressHub> hub) => _hub = hub;

        public Task SendAsync(
            string connectionId,
            string method,
            object?[] args,
            CancellationToken cancellationToken = default)
            => _hub.Clients.Client(connectionId)
                   .SendCoreAsync(method, args, cancellationToken);
    }
}

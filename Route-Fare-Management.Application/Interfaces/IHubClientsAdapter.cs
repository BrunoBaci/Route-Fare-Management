using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Route_Fare_Management.Application.Interfaces
{
    public interface IHubClientsAdapter
    {
        /// <summary>
        /// Reference to a function that sends a SignalR message to a specific connection
        /// </summary>
        Task SendAsync(
            string connectionId,
            string method,
            object?[] args,
            CancellationToken cancellationToken = default);
    }

}

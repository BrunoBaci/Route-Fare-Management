using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Route_Fare_Management.Application.Export
{
    public record ExportPricesCommand(
    Guid TourOperatorId,
    Guid SeasonId,
    string ConnectionId) : IRequest<string>;
}

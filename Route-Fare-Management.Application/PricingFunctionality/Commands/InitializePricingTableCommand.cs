using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Route_Fare_Management.Application.PricingFunctionality.Commands
{
    /// <summary>
    /// Query to create one empty PricingEntry per calendar day in the season
    /// Days that already have entries are skipped.
    /// Returns the number of rows created
    /// </summary>
    public sealed record InitializePricingTableCommand(
        Guid TourOperatorId,
        Guid RouteId,
        Guid SeasonId) : IRequest<int>;
}

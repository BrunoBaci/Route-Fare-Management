using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Route_Fare_Management.Application.PricingFunctionality.DTOs;

namespace Route_Fare_Management.Application.PricingFunctionality.Commands
{
    public record UpsertPricingCommand(
        Guid TourOperatorId,
        Guid RouteId,
        Guid SeasonId,
        List<PricingEntryUpsertDto> Entries) : IRequest<Unit>;
}

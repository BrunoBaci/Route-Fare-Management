using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Route_Fare_Management.Application.RouteFunctionality.DTOs;
using Route_Fare_Management.Application.Season;
using Route_Fare_Management.Application.TourOperator.DTOs;

namespace Route_Fare_Management.Application.PricingFunctionality.DTOs
{
    public record PricingTableDto(
        Guid TourOperatorRouteId,
        TourOperatorDto Operator,
        RouteDto Route,
        SeasonDto Season,
        List<PricingEntryDto> Entries);
}

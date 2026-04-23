using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Route_Fare_Management.Application.RouteFunctionality.DTOs;
using Route_Fare_Management.Application.Season;

namespace Route_Fare_Management.Application.TourOperator.DTOs
{
    public record TourOperatorRouteDto(
        Guid Id,
        Guid TourOperatorId,
        string TourOperatorName,
        RouteDto Route,
        SeasonDto Season,
        DateTime CreatedAt);

}

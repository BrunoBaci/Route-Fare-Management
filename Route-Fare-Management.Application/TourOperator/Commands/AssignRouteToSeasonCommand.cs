using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Route_Fare_Management.Application.TourOperator.DTOs;

namespace Route_Fare_Management.Application.TourOperator.Commands
{
    public sealed record AssignRouteToSeasonCommand(
        Guid TourOperatorId,
        Guid RouteId,
        Guid SeasonId) : IRequest<TourOperatorRouteDto>;

}

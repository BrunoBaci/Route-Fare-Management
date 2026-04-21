using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Route_Fare_Management.Application.RouteFunctionality.DTOs;

namespace Route_Fare_Management.Application.RouteFunctionality.Queries
{
    public record GetRouteByIdQuery(Guid Id) : IRequest<RouteDto>;
}

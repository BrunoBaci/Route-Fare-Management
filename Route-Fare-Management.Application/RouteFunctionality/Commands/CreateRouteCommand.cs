using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Route_Fare_Management.Application.RouteFunctionality.DTOs;
using Route_Fare_Management.Domain;

namespace Route_Fare_Management.Application.RouteFunctionality.Commands
{
    public record CreateRouteCommand(
        string Origin,
        string Destination,
        string? Description,
        List<BookingClass> BookingClasses) : IRequest<RouteDto>;

}

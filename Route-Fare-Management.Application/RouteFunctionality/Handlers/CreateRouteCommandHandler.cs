using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Route_Fare_Management.Application.Interfaces;
using Route_Fare_Management.Application.RouteFunctionality.Commands;
using Route_Fare_Management.Application.RouteFunctionality.DTOs;

namespace Route_Fare_Management.Application.RouteFunctionality.Handlers
{
    public class CreateRouteCommandHandler
        : IRequestHandler<CreateRouteCommand, RouteDto>
    {
        private readonly IRepository repository;

        public CreateRouteCommandHandler(IRepository repository)
            => this.repository = repository;

        public async Task<RouteDto> Handle(
            CreateRouteCommand request, CancellationToken cancellationToken)
        {
            var route = Domain.Route.Create(
                request.Origin,
                request.Destination,
                request.Description,
                request.BookingClasses);

            await repository.AddAndSaveAsync(route, cancellationToken);

            return route.ToDto();
        }
    }
}

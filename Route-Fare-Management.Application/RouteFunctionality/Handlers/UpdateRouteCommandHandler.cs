using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Route_Fare_Management.Application.Interfaces;
using Route_Fare_Management.Application.RouteFunctionality.Commands;
using Route_Fare_Management.Application.RouteFunctionality.DTOs;
using Route_Fare_Management.Domain.Exceptions;

namespace Route_Fare_Management.Application.RouteFunctionality.Handlers
{
    public class UpdateRouteCommandHandler
        : IRequestHandler<UpdateRouteCommand, RouteDto>
    {
        private readonly IRepository _repository;

        public UpdateRouteCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<RouteDto> Handle(
            UpdateRouteCommand request, CancellationToken cancellationToken)
        {
            var route = await _repository.UpdateRouteAsync(request.Id,
                request.Origin,
                request.Destination,
                request.Description,
                request.BookingClasses,
                cancellationToken)
                ?? throw new NotFoundException(
                    nameof(Domain.Route), request.Id);
            return route.ToDto();
        }
    }
}

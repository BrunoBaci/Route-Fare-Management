using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Route_Fare_Management.Application.Interfaces;
using Route_Fare_Management.Application.RouteFunctionality.DTOs;
using Route_Fare_Management.Application.RouteFunctionality.Queries;
using Route_Fare_Management.Domain.Exceptions;

namespace Route_Fare_Management.Application.RouteFunctionality.Handlers
{
    public class GetRouteByIdQueryHandler
        : IRequestHandler<GetRouteByIdQuery, RouteDto>
    {
        private readonly IRepository repository;

        public GetRouteByIdQueryHandler(IRepository context)
        {
            repository = context;
        }

        public async Task<RouteDto> Handle(
            GetRouteByIdQuery request, CancellationToken cancellationToken)
        {
            var route = await repository.GetRouteAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(
                    nameof(Domain.Route), request.Id);

            return route.ToDto();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Route_Fare_Management.Application.Interfaces;
using Route_Fare_Management.Application.Season;
using Route_Fare_Management.Application.PricingFunctionality.DTOs;
using Route_Fare_Management.Application.PricingFunctionality.Queries;
using Route_Fare_Management.Application.RouteFunctionality.DTOs;
using Route_Fare_Management.Domain.Exceptions;
using Route_Fare_Management.Application.TourOperator.DTOs;

namespace Route_Fare_Management.Application.PricingFunctionality.Handlers
{

    public class GetPricingQueryHandler
        : IRequestHandler<GetPricingQuery, PricingTableDto>
    {
        private readonly IRepository _repository;
        private readonly ICurrentUserService _currentUser;

        public GetPricingQueryHandler(
            IRepository repository,
            ICurrentUserService currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<PricingTableDto> Handle(
            GetPricingQuery request, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsAdmin &&
                _currentUser.TourOperatorId != request.TourOperatorId)
                throw new ForbiddenAccessException();

            var tor = await _repository.GetForPricingInitializationAsync(request.TourOperatorId, request.RouteId, request.SeasonId, cancellationToken)
                ?? throw new NotFoundException(
                    nameof(Domain.TourOperatorRoute),
                    $"Operator={request.TourOperatorId}, Route={request.RouteId}, Season={request.SeasonId}");

            var entries = tor.PricingEntries
                .OrderBy(e => e.Date)
                .Select(e => e.ToDto())
                .ToList();

            return new PricingTableDto(
                tor.Id,
                tor.TourOperator.ToDto(),
                tor.Route.ToDto(),
                tor.Season.ToDto(),
                entries);
        }
    }
}

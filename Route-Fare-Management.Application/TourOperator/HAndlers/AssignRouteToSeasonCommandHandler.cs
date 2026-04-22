using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Route_Fare_Management.Application.Interfaces;
using Route_Fare_Management.Application.TourOperator.Commands;
using Route_Fare_Management.Application.TourOperator.DTOs;
using Route_Fare_Management.Domain.Exceptions;
using Route_Fare_Management.Domain;
using Route_Fare_Management.Application.RouteFunctionality.DTOs;
using Route_Fare_Management.Application.Season;

namespace Route_Fare_Management.Application.TourOperator.HAndlers
{
    public class AssignRouteToSeasonCommandHandler
        : IRequestHandler<AssignRouteToSeasonCommand, TourOperatorRouteDto>
    {
        private readonly IRepository _repo;
        private readonly ICurrentUserService _currentUser;

        public AssignRouteToSeasonCommandHandler(
            IRepository repo,
            ICurrentUserService currentUser)
        {
            _repo = repo;
            _currentUser = currentUser;
        }

        public async Task<TourOperatorRouteDto> Handle(
            AssignRouteToSeasonCommand request, CancellationToken cancellationToken)
        {
            // Operators can only assign to their own account
            if (!_currentUser.IsAdmin &&
                _currentUser.TourOperatorId != request.TourOperatorId)
                throw new ForbiddenAccessException();

            // Check for duplicate assignment
            if (await _repo.ExistsTourOperatorRouteAsync(
                        request.TourOperatorId,
                        request.RouteId,
                        request.SeasonId,
                        cancellationToken))
            {
                    throw new DomainException(
                        "This route is already assigned to the operator for this season.");
            }
            // Validate 
            var op = await _repo.GetTourOperatorAsync(request.TourOperatorId, cancellationToken)
                ?? throw new NotFoundException(nameof(TourOperator), request.TourOperatorId);


            var route = await _repo.GetRouteAsync(request.RouteId, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.Route), request.RouteId);

            var season = await _repo.GetSeasonAsync(request.SeasonId, cancellationToken)
                ?? throw new NotFoundException(nameof(Season), request.SeasonId);

            var tor = TourOperatorRoute.Create(
                request.TourOperatorId, request.RouteId, request.SeasonId);

            await _repo.AddAndSaveAsync(tor, cancellationToken);

            return new TourOperatorRouteDto(
                tor.Id, op.Id, op.Name,
                route.ToDto(), season.ToDto(),
                tor.CreatedAt);
        }
    }

}

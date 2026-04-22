using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Route_Fare_Management.Application.Interfaces;
using Route_Fare_Management.Application.RouteFunctionality.DTOs;
using Route_Fare_Management.Application.Season;
using Route_Fare_Management.Application.TourOperator.DTOs;
using Route_Fare_Management.Application.TourOperator.Queries;
using Route_Fare_Management.Domain.Exceptions;

namespace Route_Fare_Management.Application.TourOperator.HAndlers
{
    public sealed class GetTourOperatorRoutesQueryHandler
        : IRequestHandler<GetTourOperatorRoutesQuery, List<TourOperatorRouteDto>>
    {
        private readonly IRepository _Repo;
        private readonly ICurrentUserService _currentUser;

        public GetTourOperatorRoutesQueryHandler(
            IRepository repo,
            ICurrentUserService currentUser)
        {
            _Repo = repo;
            _currentUser = currentUser;
        }

        public async Task<List<TourOperatorRouteDto>> Handle(
            GetTourOperatorRoutesQuery request, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsAdmin &&
                _currentUser.TourOperatorId != request.TourOperatorId)
                throw new ForbiddenAccessException();

            var operatorRouter = await _Repo.GetOperatorRoutesAsync(
                request.TourOperatorId,
                request.SeasonId,
                cancellationToken);

            return operatorRouter
                .Select(t => new TourOperatorRouteDto(
                    t.Id, 
                    t.TourOperatorId, 
                    t.TourOperator.Name,
                    t.Route.ToDto(),
                    t.Season.ToDto(),
                    t.CreatedAt))
                .ToList();
        }
    }

}

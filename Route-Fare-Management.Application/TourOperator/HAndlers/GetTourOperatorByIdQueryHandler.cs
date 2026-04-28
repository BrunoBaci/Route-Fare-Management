using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Route_Fare_Management.Application.Interfaces;
using Route_Fare_Management.Application.TourOperator.DTOs;
using Route_Fare_Management.Application.TourOperator.Queries;
using Route_Fare_Management.Domain.Exceptions;

namespace Route_Fare_Management.Application.TourOperator.HAndlers
{
    public sealed class GetTourOperatorByIdQueryHandler
        : IRequestHandler<GetTourOperatorByIdQuery, TourOperatorDto>
    {
        private readonly IRepository _repo;
        private readonly ICurrentUserService _currentUser;
        public GetTourOperatorByIdQueryHandler(IRepository repository, ICurrentUserService currentUser)
        { 
            _repo = repository;
            _currentUser = currentUser;
        }

        public async Task<TourOperatorDto> Handle(
            GetTourOperatorByIdQuery request, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsAdmin && //if user is not an administrator
                 _currentUser.TourOperatorId != request.Id) // and is sending a request for an operator other than himself
                throw new ForbiddenAccessException();
            var op = await _repo.GetATourOperatorAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(
                    nameof(Domain.TourOperator), request.Id);

            return op.ToDto();
        }
    }
}

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

        public GetTourOperatorByIdQueryHandler(IRepository context)
            => _repo = context;

        public async Task<TourOperatorDto> Handle(
            GetTourOperatorByIdQuery request, CancellationToken cancellationToken)
        {
            var op = await _repo.GetTourOperatorWithMembersAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(
                    nameof(Domain.TourOperator), request.Id);

            return op.ToDto();
        }
    }
}

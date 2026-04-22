using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Route_Fare_Management.Application.Interfaces;
using Route_Fare_Management.Application.TourOperator.DTOs;
using Route_Fare_Management.Application.TourOperator.Queries;

namespace Route_Fare_Management.Application.TourOperator.HAndlers
{
    public sealed class GetTourOperatorsQueryHandler
        : IRequestHandler<GetTourOperatorsQuery, List<TourOperatorDto>>
    {
        private readonly IRepository _repo;

        public GetTourOperatorsQueryHandler(IRepository repo)
            => _repo = repo;

        public async Task<List<TourOperatorDto>> Handle(
            GetTourOperatorsQuery request, CancellationToken cancellationToken)
        {
            bool activeOnly = request.ActiveOnly ?? false;
            var tourOperators = await _repo.GetTourOperatorsAsync(cancellationToken, activeOnly);
            return tourOperators.Select(t => t.ToDto()).ToList();
        }
    }
}

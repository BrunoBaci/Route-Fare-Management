using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Route_Fare_Management.Application.Interfaces;
using Route_Fare_Management.Application.Season.Queries;

namespace Route_Fare_Management.Application.Season.Handlers
{
    public class GetSeasonsQueryHandler
        : IRequestHandler<GetSeasonsQuery, List<SeasonDto>>
    {
        private readonly IRepository _repo;

        public GetSeasonsQueryHandler(IRepository context)
            => _repo = context;

        public async Task<List<SeasonDto>> Handle(
            GetSeasonsQuery request, CancellationToken cancellationToken)
        {
            var seasons = await _repo.GetSeasonsAsync(request.Year, cancellationToken);

            return seasons.Select(s => s.ToDto()).ToList();
        }
    }
}

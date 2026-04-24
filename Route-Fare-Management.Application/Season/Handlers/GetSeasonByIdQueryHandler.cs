using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Route_Fare_Management.Application.Interfaces;
using Route_Fare_Management.Application.Season.Queries;
using Route_Fare_Management.Domain.Exceptions;

namespace Route_Fare_Management.Application.Season.Handlers
{
    public class GetSeasonByIdQueryHandler
        : IRequestHandler<GetSeasonByIdQuery, SeasonDto>
    {
        private readonly IRepository repository;

        public GetSeasonByIdQueryHandler(IRepository repository)
            => this.repository = repository;

        public async Task<SeasonDto> Handle(
            GetSeasonByIdQuery request, CancellationToken cancellationToken)
        {
            var season = await repository.GetSeasonAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(
                    nameof(Domain.Season), request.Id);

            return season.ToDto();
        }
    }
}

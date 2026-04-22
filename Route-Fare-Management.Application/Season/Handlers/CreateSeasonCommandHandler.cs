using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Route_Fare_Management.Application.Interfaces;
using Route_Fare_Management.Application.Season.Commands;

namespace Route_Fare_Management.Application.Season.Handlers
{
    public sealed class CreateSeasonCommandHandler
        : IRequestHandler<CreateSeasonCommand, SeasonDto>
    {
        private readonly IRepository _repo;

        public CreateSeasonCommandHandler(IRepository _repository)
            => _repo = _repository;

        public async Task<SeasonDto> Handle(
            CreateSeasonCommand request, CancellationToken cancellationToken)
        {
            var season = Domain.Season.Create(request.Year, request.Type);
            await _repo.AddAndSaveAsync(season, cancellationToken);
            return season.ToDto();
        }
    }
}
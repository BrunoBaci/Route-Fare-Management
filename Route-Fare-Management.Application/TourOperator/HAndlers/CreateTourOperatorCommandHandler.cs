using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Route_Fare_Management.Application.Interfaces;
using Route_Fare_Management.Application.TourOperator.Commands;
using Route_Fare_Management.Application.TourOperator.DTOs;

namespace Route_Fare_Management.Application.TourOperator.HAndlers
{

    public class CreateTourOperatorCommandHandler
        : IRequestHandler<CreateTourOperatorCommand, TourOperatorDto>
    {
        private readonly IRepository _repo;

        public CreateTourOperatorCommandHandler(IRepository repos)
        { _repo = repos; }

        public async Task<TourOperatorDto> Handle(
            CreateTourOperatorCommand request, CancellationToken cancellationToken)
        {
            var op = Domain.TourOperator.Create(
                request.Name,  request.SupportedBookingClasses);

            await _repo.AddAsync(op, cancellationToken);
            await _repo.SaveChangesAsync(cancellationToken);

            // Reload with navigation properties for correct MemberCount
            var loaded = await _repo.GetATourOperatorAsync(op.Id, cancellationToken)
            ?? throw new Exception("Failed to load created TourOperator");

            return loaded.ToDto();
        }
    }

}

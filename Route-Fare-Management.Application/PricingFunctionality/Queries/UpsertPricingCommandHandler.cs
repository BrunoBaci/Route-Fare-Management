using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Route_Fare_Management.Application.Interfaces;
using Route_Fare_Management.Application.PricingFunctionality.Commands;
using Route_Fare_Management.Domain.Exceptions;
using Route_Fare_Management.Domain;

namespace Route_Fare_Management.Application.PricingFunctionality.Queries
{

    public class UpsertPricingCommandHandler
        : IRequestHandler<UpsertPricingCommand, Unit>
    {
        private readonly IRepository repository;
        private readonly ICurrentUserService _currentUser;

        public UpsertPricingCommandHandler(
            IRepository context,
            ICurrentUserService currentUser)
        {
            repository = context;
            _currentUser = currentUser;
        }

        public async Task<Unit> Handle(
            UpsertPricingCommand request, CancellationToken cancellationToken)
        {
            // Operators can only edit their own pricing
            if (!_currentUser.IsAdmin &&
                _currentUser.TourOperatorId != request.TourOperatorId)
                throw new ForbiddenAccessException();

            var tor = await repository.GetWithPricingAsync(
                request.TourOperatorId,
                request.RouteId,
                request.SeasonId,
                cancellationToken)
            ?? throw new NotFoundException(
                    nameof(TourOperatorRoute),
                    $"Operator={request.TourOperatorId}, Route={request.RouteId}, Season={request.SeasonId}");

            foreach (var dto in request.Entries)
            {
                var existing = tor.PricingEntries
                    .FirstOrDefault(e => e.Date == dto.Date);

                if (existing is null)
                {
                    // Create new row
                    var newEntry = PricingEntry.Create(
                        tor.Id, dto.Date,
                        dto.EconomyPrice, dto.BusinessPrice, dto.FirstClassPrice,
                        dto.EconomySeats, dto.BusinessSeats, dto.FirstClassSeats);
                    await repository.AddAsync(newEntry, cancellationToken);
                }
                else
                {
                    // Update existing row
                    existing.UpdatePricing(
                        dto.EconomyPrice, dto.BusinessPrice, dto.FirstClassPrice,
                        dto.EconomySeats, dto.BusinessSeats, dto.FirstClassSeats);
                }
            }

            await repository.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}

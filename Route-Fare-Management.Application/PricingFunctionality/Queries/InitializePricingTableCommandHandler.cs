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

    public class InitializePricingTableCommandHandler
        : IRequestHandler<InitializePricingTableCommand, int>
    {
        private readonly IRepository _repository;
        private readonly ICurrentUserService _currentUser;

        public InitializePricingTableCommandHandler(
            IRepository context,
            ICurrentUserService currentUser)
        {
            _repository = context;
            _currentUser = currentUser;
        }

        public async Task<int> Handle(
            InitializePricingTableCommand request, CancellationToken cancellationToken)
        {
            // Admin can create for everyone
            // Operators can create only for themselves
            if (!_currentUser.IsAdmin &&
                _currentUser.TourOperatorId != request.TourOperatorId)
                throw new ForbiddenAccessException();

            var tor = await _repository.GetForPricingInitializationAsync(request.TourOperatorId,
                request.RouteId,
                request.SeasonId, cancellationToken)
                ?? throw new NotFoundException(
                    nameof(TourOperatorRoute),
                    $"Operator={request.TourOperatorId}, Route={request.RouteId}, Season={request.SeasonId}");

            // Build a set of dates that already have rows 
            var existingDates = tor.PricingEntries
                .Select(e => e.Date)
                .ToHashSet();

            // Create one empty row for every season day that doesn't have one yet
            var newEntries = tor.Season
                .AllDates()
                .Where(date => !existingDates.Contains(date))
                .Select(date => PricingEntry.Create(tor.Id, date))
                .ToList();

            if (newEntries.Any())
            {
                await _repository.AddPricingEntriesAsync(newEntries, cancellationToken);
            }

            return newEntries.Count;
        }
    }
}

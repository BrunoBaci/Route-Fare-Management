using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Route_Fare_Management.Application.Interfaces;
using Route_Fare_Management.Application.TourOperator.Commands;
using Route_Fare_Management.Application.TourOperator.DTOs;
using Route_Fare_Management.Domain.Exceptions;

namespace Route_Fare_Management.Application.TourOperator.HAndlers
{
    public sealed class UpdateTourOperatorCommandHandler
        : IRequestHandler<UpdateTourOperatorCommand, TourOperatorDto>
    {
        private readonly IRepository repository;
        private readonly ICurrentUserService _currentUser;

        public UpdateTourOperatorCommandHandler(IRepository repository, ICurrentUserService currentUser)
        {
            this.repository = repository; 
            _currentUser = currentUser;
        } 

        public async Task<TourOperatorDto> Handle(
            UpdateTourOperatorCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsAdmin &&
                _currentUser.TourOperatorId != request.Id)
                throw new ForbiddenAccessException();
            var op = await repository.GetATourOperatorAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(
                    nameof(Domain.TourOperator), request.Id);

            op.Update(request.Name, request.SupportedBookingClasses);
            await repository.SaveChangesAsync(cancellationToken);

            return op.ToDto();
        }
    }

}

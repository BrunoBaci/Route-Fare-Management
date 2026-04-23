using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Route_Fare_Management.Application.Interfaces;
using Route_Fare_Management.Domain;

namespace Route_Fare_Management.Application.Auth
{
    public class RegisterCommandHandler
        : IRequestHandler<RegisterCommand, AuthResponseDto>
    {
        private readonly IRepository _context;
        private readonly IJwtService _jwt;
        private readonly IPasswordHasher _hasher;

        public RegisterCommandHandler(
            IRepository context,
            IJwtService jwt,
            IPasswordHasher hasher)
        {
            _context = context;
            _jwt = jwt;
            _hasher = hasher;
        }

        public async Task<AuthResponseDto> Handle(
            RegisterCommand request, CancellationToken cancellationToken)
        {
            var hash = _hasher.Hash(request.Password);

            bool isAdmin = request.Role == UserRole.Admin;
            User user = null;
            if (isAdmin)
            {
                user = User.CreateAdmin(request.Email, hash, request.FirstName, request.LastName);
            }
            else
            {
                var tour = Domain.TourOperator.Create(request.FirstName, Enumerable.Empty<BookingClass>());
               user =  User.CreateTourOperatorMember(
                    request.Email, hash,
                    request.FirstName, request.LastName,
                    tour.Id);
                await _context.AddAsync(tour, cancellationToken);
            }


            await _context.AddAndSaveAsync(user, cancellationToken);

            var token = _jwt.GenerateToken(user);

            return new AuthResponseDto(
                user.Id, user.Email, user.FirstName, user.LastName,
                user.Role.ToString(), user.TourOperatorId,
                token, DateTime.UtcNow.AddHours(24));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Route_Fare_Management.Application.Interfaces;
using Route_Fare_Management.Domain;

namespace Route_Fare_Management.Application
{
    public sealed class RegisterCommandHandler
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

            var user = request.Role == UserRole.Admin
                ? User.CreateAdmin(request.Email, hash, request.FirstName, request.LastName)
                : User.CreateTourOperatorMember(
                    request.Email, hash,
                    request.FirstName, request.LastName,
                    request.TourOperatorId!.Value);

            await _context.AddUserAsync(user, cancellationToken);

            var token = _jwt.GenerateToken(user);

            return new AuthResponseDto(
                user.Id, user.Email, user.FirstName, user.LastName,
                user.Role.ToString(), user.TourOperatorId,
                token, DateTime.UtcNow.AddHours(24));
        }
    }
}

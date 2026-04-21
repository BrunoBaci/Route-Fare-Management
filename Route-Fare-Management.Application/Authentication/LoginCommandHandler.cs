using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Route_Fare_Management.Application.Interfaces;
using Route_Fare_Management.Domain;
using Route_Fare_Management.Domain.Exceptions;

namespace Route_Fare_Management.Application.Auth
{

    public sealed class LoginCommandHandler
        : IRequestHandler<LoginCommand, AuthResponseDto>
    {
        private readonly IRepository _context;
        private readonly IJwtService _jwt;
        private readonly IPasswordHasher _hasher;

        public LoginCommandHandler(
            IRepository context,
            IJwtService jwt,
            IPasswordHasher hasher)
        {
            _context = context;
            _jwt = jwt;
            _hasher = hasher;
        }

        public async Task<AuthResponseDto> Handle(
            LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.GetUserAsync(request.Email, cancellationToken)
                ?? throw new NotFoundException(
                    nameof(User), request.Email);

            if (!_hasher.Verify(request.Password, user.PasswordHash))
                throw new DomainException("Invalid email or password.");

            var token = _jwt.GenerateToken(user);

            return new AuthResponseDto(
                user.Id, user.Email, user.FirstName, user.LastName,
                user.Role.ToString(), user.TourOperatorId,
                token, DateTime.UtcNow.AddHours(24));
        }
    }

}

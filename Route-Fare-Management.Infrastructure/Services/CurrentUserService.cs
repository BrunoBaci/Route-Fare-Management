using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Route_Fare_Management.Application.Interfaces;
using Route_Fare_Management.Domain;

namespace Route_Fare_Management.Infrastructure.Services
{

    public sealed class CurrentUserService : ICurrentUserService
    {
        private readonly ClaimsPrincipal? _principal;

        public CurrentUserService(IHttpContextAccessor accessor)
            => _principal = accessor.HttpContext?.User;

        public bool IsAuthenticated
            => _principal?.Identity?.IsAuthenticated ?? false;

        public Guid UserId
        {
            get
            {
                var value = _principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return Guid.TryParse(value, out var id) ? id : Guid.Empty;
            }
        }

        public string Email
            => _principal?.FindFirst(ClaimTypes.Email).Value ?? string.Empty;

        public UserRole Role
        {
            get
            {
                var value = _principal?.FindFirst(ClaimTypes.Role).Value;
                return Enum.TryParse<UserRole>(value, out var role)
                    ? role
                    : UserRole.TourOperatorMember;
            }
        }

        public Guid? TourOperatorId
        {
            get
            {
                var value = _principal?.FindFirst("TourOperatorId").Value;
                return Guid.TryParse(value, out var id) ? id : null;
            }
        }

        public bool IsAdmin => Role == UserRole.Admin;
    }
}

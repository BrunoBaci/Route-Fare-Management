using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Route_Fare_Management.Domain;

namespace Route_Fare_Management.Application.Interfaces
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        string Email { get; }
        UserRole Role { get; }
        Guid? TourOperatorId { get; }
        bool IsAdmin { get; }
        bool IsAuthenticated { get; }
    }
}

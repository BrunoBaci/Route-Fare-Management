using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Route_Fare_Management.Application
{
    public record AuthResponseDto(
        Guid UserId,
        string Email,
        string FirstName,
        string LastName,
        string Role,
        Guid? TourOperatorId,
        string Token,
        DateTime ExpiresAt);
}

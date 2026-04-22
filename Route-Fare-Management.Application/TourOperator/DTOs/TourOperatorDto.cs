using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Route_Fare_Management.Application.TourOperator.DTOs
{
    public record TourOperatorDto(
        Guid Id,
        string Name,
        string Code,
        bool IsActive,
        List<string> SupportedBookingClasses,
        int MemberCount,
        DateTime CreatedAt);
}

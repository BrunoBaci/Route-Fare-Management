using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Route_Fare_Management.Application.RouteFunctionality.DTOs
{
    public record RouteDto(
        Guid Id,
        string Origin,
        string Destination,
        string? Description,
        List<string> AvailableBookingClasses,
        bool IsActive,
        DateTime CreatedAt);
}

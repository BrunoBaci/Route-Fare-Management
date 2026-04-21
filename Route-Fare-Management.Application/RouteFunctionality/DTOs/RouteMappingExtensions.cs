using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Route_Fare_Management.Application.RouteFunctionality.DTOs
{
    public static class RouteMappingExtensions
    {
        /// <summary>
        /// Helper method that maps a materialised Route entity to RouteDto.
        /// </summary>
        public static RouteDto ToDto(this Domain.Route r) =>
            new(r.Id, r.Origin, r.Destination, r.Description,
                r.AvailableBookingClasses.Select(bc => bc.ToString()).ToList(),
                r.IsActive, r.CreatedAt);
    }
}

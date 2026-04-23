using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Route_Fare_Management.Domain
{
    public class Route : BaseEntity
    {
        public string Origin { get; private set; } = default!;
        public string Destination { get; private set; } = default!;
        public string? Description { get; private set; }
        public bool IsActive { get; private set; } = true;

        // Private backing field — EF Core accesses this directly via HasField()
        private List<BookingClass> _availableBookingClasses = new();

        public IReadOnlyCollection<BookingClass> AvailableBookingClasses
            => _availableBookingClasses.AsReadOnly();

        public ICollection<TourOperatorRoute> TourOperatorRoutes { get; private set; }
            = new List<TourOperatorRoute>();

        // Required by EF Core
        private Route() { }

        public static Route Create(
            string origin, string destination,
            string? description, IEnumerable<BookingClass> bookingClasses)
        {
            var r = new Route
            {
                Origin = origin.Trim(),
                Destination = destination.Trim(),
                Description = description?.Trim()
            };
            r._availableBookingClasses = bookingClasses.Distinct().ToList();
            return r;
        }

        public void Update(
            string origin, string destination,
            string? description, IEnumerable<BookingClass> bookingClasses)
        {
            Origin = origin.Trim();
            Destination = destination.Trim();
            Description = description?.Trim();
            _availableBookingClasses = bookingClasses.Distinct().ToList();
            SetUpdatedAt();
        }

    }

}

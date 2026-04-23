using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Route_Fare_Management.Domain
{

    public sealed class TourOperator : BaseEntity
    {
        public string Name { get; private set; } = default!;
        public bool IsActive { get; private set; } = true;

        public Guid? UserId { get; private set; }
        public User OwnerUser { get; private set; } /*= default!;*/

        private List<BookingClass> _supportedBookingClasses = new();

        public IReadOnlyCollection<BookingClass> SupportedBookingClasses
            => _supportedBookingClasses.AsReadOnly();

        public ICollection<TourOperatorRoute> TourOperatorRoutes { get; private set; }
            = new List<TourOperatorRoute>();

        // Required by EF Core
        private TourOperator() { }

        public static TourOperator Create(
            string name, IEnumerable<BookingClass> bookingClasses)
        {
            if (bookingClasses == null || bookingClasses.Count() == 0)
            {
                bookingClasses = new List<BookingClass>() 
                {
                    0
                };
            }
            var op = new TourOperator
            {
                Name = name.Trim(),
            };
            op._supportedBookingClasses = bookingClasses.Distinct().ToList();
            return op;
        }

        public void Update(string name, IEnumerable<BookingClass> bookingClasses)
        {
            Name = name.Trim();
            _supportedBookingClasses = bookingClasses.Distinct().ToList();
            SetUpdatedAt();
        }

        public void Deactivate() { IsActive = false; SetUpdatedAt(); }
        public void Activate() { IsActive = true; SetUpdatedAt(); }
    }

}

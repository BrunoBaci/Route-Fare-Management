using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Route_Fare_Management.Domain
{
    public class Season : BaseEntity
    {
        public int Year { get; private set; }
        public SeasonType Type { get; private set; }
        public DateOnly StartDate { get; private set; }
        public DateOnly EndDate { get; private set; }

        public ICollection<TourOperatorRoute> TourOperatorRoutes { get; private set; }
            = new List<TourOperatorRoute>();

        // Required by EF Core
        private Season() { }

        public static Season Create(int year, SeasonType type)
        {
            // Business rule: Winter = Jan–Jun, Summer = Jul–Dec
            var (start, end) = type == SeasonType.Winter
                ? (new DateOnly(year, 1, 1), new DateOnly(year, 6, 30))
                : (new DateOnly(year, 7, 1), new DateOnly(year, 12, 31));

            return new Season
            {
                Year = year,
                Type = type,
                StartDate = start,
                EndDate = end
            };
        }

        // Computed — not stored in DB (Ignored in EF config)
        public string DisplayName => $"{Type} {Year}";
        public int TotalDays => EndDate.DayNumber - StartDate.DayNumber + 1;

        /// <summary>
        /// Enumerates every calendar date within this season (lazy/streaming).
        /// Used to initialise pricing tables.
        /// </summary>
        public IEnumerable<DateOnly> AllDates()
        {
            var current = StartDate;
            while (current <= EndDate)
            {
                yield return current;
                current = current.AddDays(1);
            }
        }
    }

}

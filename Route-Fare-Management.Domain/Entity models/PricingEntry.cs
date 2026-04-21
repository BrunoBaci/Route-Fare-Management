using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Route_Fare_Management.Domain
{

    /// <summary>
    /// One row in the pricing table: a single calendar day for a specific
    /// TourOperator + Route + Season combination. All price/seat fields are
    /// nullable — null means "not configured yet" which is valid.
    /// </summary>
    public class PricingEntry : BaseEntity
    {
        public Guid TourOperatorRouteId { get; private set; }
        public DateOnly Date { get; private set; }
        public DayOfWeek DayOfWeek { get; private set; }

        // Prices per booking class — nullable means not yet configured
        public decimal? EconomyPrice { get; private set; }
        public decimal? BusinessPrice { get; private set; }
        public decimal? FirstClassPrice { get; private set; }

        // Seats requested per booking class — nullable means not yet configured
        public int? EconomySeats { get; private set; }
        public int? BusinessSeats { get; private set; }
        public int? FirstClassSeats { get; private set; }

        // Navigation property
        public TourOperatorRoute TourOperatorRoute { get; private set; } = default!;

        // Required by EF Core
        private PricingEntry() { }

        public static PricingEntry Create(
            Guid tourOperatorRouteId,
            DateOnly date,
            decimal? economyPrice = null,
            decimal? businessPrice = null,
            decimal? firstClassPrice = null,
            int? economySeats = null,
            int? businessSeats = null,
            int? firstClassSeats = null)
            => new()
            {
                TourOperatorRouteId = tourOperatorRouteId,
                Date = date,
                // DayOfWeek derived from date — stored for fast filtering
                DayOfWeek = date.DayOfWeek,
                EconomyPrice = economyPrice,
                BusinessPrice = businessPrice,
                FirstClassPrice = firstClassPrice,
                EconomySeats = economySeats,
                BusinessSeats = businessSeats,
                FirstClassSeats = firstClassSeats
            };

        public void UpdatePricing(
            decimal? economyPrice,
            decimal? businessPrice,
            decimal? firstClassPrice,
            int? economySeats,
            int? businessSeats,
            int? firstClassSeats)
        {
            EconomyPrice = economyPrice;
            BusinessPrice = businessPrice;
            FirstClassPrice = firstClassPrice;
            EconomySeats = economySeats;
            BusinessSeats = businessSeats;
            FirstClassSeats = firstClassSeats;
            SetUpdatedAt();
        }
    }

}

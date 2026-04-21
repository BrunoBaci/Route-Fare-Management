using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Route_Fare_Management.Domain
{
    public class TourOperatorRoute : BaseEntity
    {
        public Guid TourOperatorId { get; private set; }
        public Guid RouteId { get; private set; }
        public Guid SeasonId { get; private set; }

        // Navigation properties
        public TourOperator TourOperator { get; private set; } = default!;
        public Route Route { get; private set; } = default!;
        public Season Season { get; private set; } = default!;

        public ICollection<PricingEntry> PricingEntries { get; private set; }
            = new List<PricingEntry>();

        // Required by EF Core
        private TourOperatorRoute() { }

        public static TourOperatorRoute Create(
            Guid tourOperatorId, Guid routeId, Guid seasonId)
            => new()
            {
                TourOperatorId = tourOperatorId,
                RouteId = routeId,
                SeasonId = seasonId
            };
    }
}

using Route_Fare_Management.Domain;

namespace Route_Fare_Management.Application.PricingFunctionality.DTOs
{
    public static class PricingMappingExtensions
    {
        public static PricingEntryDto ToDto(this PricingEntry e) =>
            new(e.Id, e.Date, e.DayOfWeek.ToString(),
                e.EconomyPrice, e.BusinessPrice, e.FirstClassPrice,
                e.EconomySeats, e.BusinessSeats, e.FirstClassSeats);
    }
}

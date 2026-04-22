using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Route_Fare_Management.Application.PricingFunctionality.DTOs
{
    public record PricingEntryUpsertDto(
        DateOnly Date,
        decimal? EconomyPrice,
        decimal? BusinessPrice,
        decimal? FirstClassPrice,
        int? EconomySeats,
        int? BusinessSeats,
        int? FirstClassSeats);
}

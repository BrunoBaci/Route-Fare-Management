using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Route_Fare_Management.Application.Season
{
    public record SeasonDto(
        Guid Id,
        int Year,
        string Type,
        DateOnly StartDate,
        DateOnly EndDate,
        string DisplayName,
        int TotalDays,
        DateTime CreatedAt);
}

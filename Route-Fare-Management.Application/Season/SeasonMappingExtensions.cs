namespace Route_Fare_Management.Application.Season
{
    public static class SeasonMappingExtensions
    {
        public static SeasonDto ToDto(this Domain.Season s) =>
            new(s.Id, s.Year, s.Type.ToString(),
                s.StartDate, s.EndDate,
                s.DisplayName, s.TotalDays,
                s.CreatedAt);
    }
}

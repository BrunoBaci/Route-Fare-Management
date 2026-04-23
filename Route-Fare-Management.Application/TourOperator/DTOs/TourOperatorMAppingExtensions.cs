namespace Route_Fare_Management.Application.TourOperator.DTOs
{
    public static class TourOperatorMappingExtensions
    {
        public static TourOperatorDto ToDto(this Domain.TourOperator op)
        {
            return new(op.Id, op.Name, op.IsActive,
                op.SupportedBookingClasses.Select(bc => bc.ToString()).ToList(),
                op.Members.Count,
                op.CreatedAt);
        }
    }
}


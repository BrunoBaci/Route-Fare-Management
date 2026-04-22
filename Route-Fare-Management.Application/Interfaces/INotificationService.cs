namespace Route_Fare_Management.Application.Interfaces
{
    public interface INotificationService
    {
        Task SendProgressAsync(
        string connectionId, int progress, string message,
        CancellationToken cancellationToken = default);

        Task SendCompletedAsync(
            string connectionId, string fileUrl,
            CancellationToken cancellationToken = default);

        Task SendErrorAsync(
            string connectionId, string error,
            CancellationToken cancellationToken = default);
    }
}

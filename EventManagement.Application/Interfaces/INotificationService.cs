namespace EventManagement.Application.Interfaces
{
    public interface INotificationService
    {
        Task NotifyParticipantsAsync(Guid eventId, bool isDateChanged, bool isLocationChanged);
    }
}

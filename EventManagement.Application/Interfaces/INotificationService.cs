

namespace EventManagement.Application.Services
{
    public interface INotificationService
    {
        Task NotifyParticipantsAsync(Guid eventId, bool isDateChanged, bool isLocationChanged);
    }
}

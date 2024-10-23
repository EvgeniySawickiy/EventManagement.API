using EventManagement.Application.Services;
using EventManagement.Application.Use_Cases.ParticipantUseCases;
using EventManagement.Core.Interfaces.Repositories;

namespace EventManagement.Infrastructure.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly GetParticipantsByEventIdUseCase _getParticipantsByEventIdUseCase;

        public NotificationService(IUnitOfWork unitOfWork, IEmailService emailService, GetParticipantsByEventIdUseCase getParticipantsByEventIdUseCase)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _getParticipantsByEventIdUseCase = getParticipantsByEventIdUseCase;
        }

        public async Task NotifyParticipantsAsync(Guid eventId, bool isDateChanged, bool isLocationChanged)
        {
            var participants = await _getParticipantsByEventIdUseCase.ExecuteAsync(eventId);

            foreach (var participant in participants)
            {
                var subject = "Important: Event Details Updated";
                var message = BuildMessage(isDateChanged, isLocationChanged);
                await _emailService.SendEmailAsync(participant.Email, subject, message);
            }
        }

        private string BuildMessage(bool isDateChanged, bool isLocationChanged)
        {
            var message = "Dear Participant, the event you are registered for has been updated:\n";
            if (isDateChanged)
            {
                message += "- The event date has been changed.\n";
            }
            if (isLocationChanged)
            {
                message += "- The event location has been changed.\n";
            }
            return message;
        }
    }
}

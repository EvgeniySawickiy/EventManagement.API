
namespace EventManagement.Application.DTO.Request
{
    public class RegisterParticipantToEventRequestDTO
    {
        public Guid UserId { get; set; }
        public Guid EventId { get; set; }
    }
}
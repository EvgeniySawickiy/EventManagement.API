
namespace EventManagement.Application.DTO.Response
{
    public class ParticipantResponseDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Email { get; set; }

        public Guid UserId { get; set; }
    }
}
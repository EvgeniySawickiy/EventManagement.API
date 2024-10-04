
namespace EventManagement.Core.Entity
{
    public class Participant
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Email { get; set; }

        public Guid UserId { get; set; } 
        public User User { get; set; }

        public ICollection<EventParticipant> EventParticipants { get; set; }
    }
}
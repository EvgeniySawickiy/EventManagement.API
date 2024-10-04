
namespace EventManagement.Core.Entity
{
    public class Event
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; }
        public int MaxParticipants { get; set; }

        public string Category { get; set; }

        public Guid? ImageId { get; set; }
        public Image Image { get; set; }

        public ICollection<EventParticipant> EventParticipants { get; set; }
    }
}
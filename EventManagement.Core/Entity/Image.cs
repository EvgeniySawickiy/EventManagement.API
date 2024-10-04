
namespace EventManagement.Core.Entity
{
    public class Image
    {
        public Guid Id { get; set; }
        public string FilePath { get; set; }

        public Guid? EventId { get; set; }
        public Event Event { get; set; }
    }
}

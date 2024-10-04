using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement.Core.Entity
{
    public class EventParticipant
    {
        public Guid Id { get; set; }

        public Guid EventId { get; set; }
        public Event Event { get; set; }

        public Guid ParticipantId { get; set; }
        public Participant Participant { get; set; }

        public DateTime RegistrationDate { get; set; }
    }
}
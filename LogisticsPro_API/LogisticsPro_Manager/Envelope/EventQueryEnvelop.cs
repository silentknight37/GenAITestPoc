using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Envelope
{
    public class EventQueryEnvelop
    {
        public EventQueryEnvelop(List<Event> events)
        {
            this.Events = events;
        }

        public List<Event> Events { get; set; }
    }
}

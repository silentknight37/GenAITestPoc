using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Query
{
    public class EventEventQuery : IRequest<EventQueryEnvelop>
    {
        public EventEventQuery(int userId) 
        {
            UserId = userId;
        }

        public int UserId { get; set; }
    }
}

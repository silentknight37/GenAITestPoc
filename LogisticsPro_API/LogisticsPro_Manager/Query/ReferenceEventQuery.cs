using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Query
{
    public class ReferenceEventQuery : IRequest<ReferanceQueryEnvelop>
    {
        public ReferenceEventQuery(int userId) 
        {
            UserId = userId;
        }

        public int UserId { get; set; }
    }
}

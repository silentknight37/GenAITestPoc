using LogisticsPro_Common.DTO;
using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Query
{
    public class JobCardReferenceEventQuery : IRequest<JobCardReferanceQueryEnvelop>
    {
        public JobCardReferenceEventQuery(int userId, int customerId) 
        {
            UserId = userId;
            CustomerId = customerId;
        }

        public int UserId { get; set; }
        public int CustomerId { get; set; }
    }
}

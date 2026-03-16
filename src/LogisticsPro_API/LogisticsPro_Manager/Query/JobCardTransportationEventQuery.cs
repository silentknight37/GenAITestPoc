using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Query
{
    public class JobCardTransportationEventQuery : IRequest<JobCardTransportationQueryEnvelop>
    {
        public JobCardTransportationEventQuery(int userId, int id)
        {
            this.UserId = userId;
            this.Id = id;
        }

        public int UserId { get; set; }
        public int Id { get; set; }
    }
}

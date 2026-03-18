using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Query
{
    public class JobCardHotelsEventQuery : IRequest<JobCardHotelsQueryEnvelop>
    {
        public JobCardHotelsEventQuery(int userId, int jobCardId)
        {
            this.UserId = userId;
            this.JobCardId = jobCardId;
        }

        public int UserId { get; set; }
        public int JobCardId { get; set; }
    }
}

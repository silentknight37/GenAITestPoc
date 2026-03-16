using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Query
{
    public class JobCardEventQuery : IRequest<JobCardQueryEnvelop>
    {
        public JobCardEventQuery(int userId, int jobCardId)
        {
            this.UserId = userId;
            this.JobCardId = jobCardId;
        }

        public int UserId { get; set; }
        public int JobCardId { get; set; }
    }
}

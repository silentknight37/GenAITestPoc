using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Query
{
    public class JobCardVisasEventQuery : IRequest<JobCardVisasQueryEnvelop>
    {
        public JobCardVisasEventQuery(int userId, int jobCardId)
        {
            this.UserId = userId;
            this.JobCardId = jobCardId;
        }

        public int UserId { get; set; }
        public int JobCardId { get; set; }
    }
}

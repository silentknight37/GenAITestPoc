using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Query
{
    public class JobCardVisaEventQuery : IRequest<JobCardVisaQueryEnvelop>
    {
        public JobCardVisaEventQuery(int userId, int id)
        {
            this.UserId = userId;
            this.Id = id;
        }

        public int UserId { get; set; }
        public int Id { get; set; }
    }
}

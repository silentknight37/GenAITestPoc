using LogisticsPro_Common.DTO;
using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Query
{
    public class JobCardMiscellaneousEventQuery : IRequest<JobCardMiscellaneousQueryEnvelop>
    {
        public JobCardMiscellaneousEventQuery(int userId, int id)
        {
            this.UserId = userId;
            this.Id = id;
        }

        public int UserId { get; set; }
        public int Id { get; set; }
    }
}

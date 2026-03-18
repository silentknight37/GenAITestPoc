
using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Query
{
    public class BatchItemEventQuery : IRequest<BatchItemQueryEnvelop>
    {
        public BatchItemEventQuery(int userId, DateTime? batchDate) 
        {
            this.UserId = userId;
            this.BatchDate = batchDate;
        }

        public int UserId { get; set; }
        public DateTime? BatchDate { get; set; }
    }
}

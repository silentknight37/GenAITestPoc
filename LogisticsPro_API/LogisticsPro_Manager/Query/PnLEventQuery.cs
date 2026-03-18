using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Query
{
    public class PnLEventQuery : IRequest<PnLEnvelop>
    {
        public PnLEventQuery(int userId, List<int>? customerId, DateTime? dateFrom, DateTime? dateTo, string? jobCardNumber) 
        {
            this.UserId = userId;
            this.CustomerId = customerId;
            this.DateFrom = dateFrom;
            this.DateTo = dateTo;
            this.JobCardNumber = jobCardNumber;
        }

        public int UserId { get; set; }
        public List<int>? CustomerId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? JobCardNumber { get; set; }
    }
}

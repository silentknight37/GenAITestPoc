
using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Query
{
    public class JobCardItemByCustomerIdEventQuery : IRequest<JobCardItemQueryEnvelop>
    {
        public JobCardItemByCustomerIdEventQuery(int userId, int customerId, DateTime? fromDate, DateTime? toDate,string? jobCardNumber) 
        {
            this.UserId = userId;
            this.CustomerId = customerId;
            this.FromDate = fromDate;
            this.ToDate = toDate;
            this.JobCardNumber = jobCardNumber;
        }

        public int UserId { get; set; }
        public int CustomerId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? JobCardNumber { get;set; }
    }
}

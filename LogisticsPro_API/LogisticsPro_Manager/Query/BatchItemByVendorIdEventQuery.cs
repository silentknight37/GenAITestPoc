
using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Query
{
    public class BatchItemByVendorIdEventQuery : IRequest<BatchItemQueryEnvelop>
    {
        public BatchItemByVendorIdEventQuery(int userId, int vendorId, DateTime? fromDate, DateTime? toDate) 
        {
            this.UserId = userId;
            this.VendorId = vendorId;
            this.FromDate = fromDate;
            this.ToDate = toDate;
        }

        public int UserId { get; set; }
        public int VendorId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}

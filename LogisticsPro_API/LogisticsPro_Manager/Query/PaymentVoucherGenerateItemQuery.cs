
using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Query
{
    public class PaymentVoucherGenerateItemQuery : IRequest<PaymentVoucherGenerateItemEnvelop>
    {
        public PaymentVoucherGenerateItemQuery(int userId, int vendorId, DateTime? fromDate, DateTime? toDate,string? jobCardNumber) 
        {
            this.UserId = userId;
            this.VendorId = vendorId;
            this.FromDate = fromDate;
            this.ToDate = toDate;
            this.JobCardNumber = jobCardNumber;
        }

        public int UserId { get; set; }
        public int VendorId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? JobCardNumber { get; set;}
    }
}

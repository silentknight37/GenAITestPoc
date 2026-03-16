using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Query
{
    public class PaymentVoucherEventQuery : IRequest<PaymentVoucherEnvelop>
    {
        public PaymentVoucherEventQuery(bool isFirstLoad,int userId, string? paymentVoucherCode, string? invoiceNo, List<int>? vendorId, DateTime? paymentVoucherDateFrom, DateTime? paymentVoucherDateTo, string? jobCardNumber) 
        {
            IsFirstLoad = isFirstLoad;
            this.UserId = userId;
            this.PaymentVoucherCode = paymentVoucherCode;
            this.InvoiceNo = invoiceNo;
            this.VendorId = vendorId;
            this.PaymentVoucherDateFrom = paymentVoucherDateFrom;
            this.PaymentVoucherDateTo = paymentVoucherDateTo;
            this.JobCardNumber = jobCardNumber;
        }
        public bool IsFirstLoad { get; set; }
        public int UserId { get; set; }
        public string PaymentVoucherCode { get; set; }
        public string InvoiceNo { get; set; }
        public List<int>? VendorId { get; set; }
        public DateTime? PaymentVoucherDateFrom { get; set; }
        public DateTime? PaymentVoucherDateTo { get; set; }
        public string? JobCardNumber { get; set; }
    }
}

using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Query
{
    public class InvoiceEventQuery : IRequest<InvoiceEnvelop>
    {
        public InvoiceEventQuery(bool isFirstLoad, int userId, string? invoiceCode, List<int>? customerId, DateTime? invoiceDateFrom, DateTime? invoiceDateTo, DateTime? invoiceDueDateFrom, DateTime? invoiceDueDateTo, List<int>? statusId,string? jobCardNumber) 
        {
            IsFirstLoad = isFirstLoad;
            this.UserId = userId;
            this.InvoiceCode = invoiceCode;
            this.CustomerId = customerId;
            this.InvoiceDateFrom = invoiceDateFrom;
            this.InvoiceDateTo = invoiceDateTo;
            this.InvoiceDueDateFrom = invoiceDueDateFrom;
            this.InvoiceDueDateTo = invoiceDueDateTo;
            this.StatusId = statusId;
            this.JobCardNumber = jobCardNumber;
        }
        public bool IsFirstLoad { get; set; }
        public int UserId { get; set; }
        public string? InvoiceCode { get; set; }
        public List<int>? CustomerId { get; set; }
        public DateTime? InvoiceDateFrom { get; set; }
        public DateTime? InvoiceDateTo { get; set; }
        public DateTime? InvoiceDueDateFrom { get; set; }
        public DateTime? InvoiceDueDateTo { get; set; }
        public List<int>? StatusId { get; set; }
        public string? JobCardNumber { get; set; }
    }
}

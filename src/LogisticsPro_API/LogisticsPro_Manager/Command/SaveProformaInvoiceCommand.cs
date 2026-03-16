using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Command
{
    public class SaveProformaInvoiceCommand : IRequest<RequestSaveEnvelop>
    {
        public SaveProformaInvoiceCommand(int customerId, string invoiceDateStn,string invoiceDueDateStn, decimal invoiceAmount,int jobCardId, string description, int userId) 
        {
            this.CustomerId = customerId;
            this.InvoiceDateStn = invoiceDateStn;
            this.InvoiceDueDateStn = invoiceDueDateStn;
            this.InvoiceAmount = invoiceAmount;
            this.JobCardId = jobCardId;
            this.Description = description;
            this.UserId = userId;
        }
        public int UserId { get; set; }
        public int CustomerId { get; set; }
        public string InvoiceDateStn { get; set; }
        public string InvoiceDueDateStn { get; set; }
        public decimal InvoiceAmount { get; set; }
        public int JobCardId { get; set; }
        public string Description { get; set; }

        public DateTime? InvoiceDate
        {
            get
            {
                return InvoiceDateStn != null ? DateTimeOffset.Parse(InvoiceDateStn).LocalDateTime : null;
            }
        }

        public DateTime? InvoiceDueDate
        {
            get
            {
                return InvoiceDueDateStn != null ? DateTimeOffset.Parse(InvoiceDueDateStn).LocalDateTime : null;
            }
        }
    }
}

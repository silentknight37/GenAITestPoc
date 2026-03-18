using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Query
{
    public class ReceiptsByInvoiceIdEventQuery : IRequest<ReceiptsEnvelop>
    {
        public ReceiptsByInvoiceIdEventQuery(int userId, int invoiceId) 
        {
            this.UserId = userId;
            this.InvoiceId = invoiceId;
        }

        public int UserId { get; set; }
        public int InvoiceId { get; set; }
    }
}

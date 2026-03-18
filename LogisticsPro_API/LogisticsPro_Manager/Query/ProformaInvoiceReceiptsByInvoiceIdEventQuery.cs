using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Query
{
    public class ProformaInvoiceReceiptsByInvoiceIdEventQuery : IRequest<ProformaInvoiceReceiptsEnvelop>
    {
        public ProformaInvoiceReceiptsByInvoiceIdEventQuery(int userId, int invoiceId) 
        {
            this.UserId = userId;
            this.InvoiceId = invoiceId;
        }

        public int UserId { get; set; }
        public int InvoiceId { get; set; }
    }
}

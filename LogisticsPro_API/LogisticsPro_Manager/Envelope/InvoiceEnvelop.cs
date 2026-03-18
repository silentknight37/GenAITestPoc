using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Envelope
{
    public class InvoiceEnvelop
    {
        public InvoiceEnvelop(List<Invoice> invoices)
        {
            this.Invoices = invoices;
        }

        public List<Invoice> Invoices { get; set; }
    }
}

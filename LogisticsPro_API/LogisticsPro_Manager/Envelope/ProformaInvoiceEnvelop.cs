using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Envelope
{
    public class ProformaInvoiceEnvelop
    {
        public ProformaInvoiceEnvelop(List<ProformaInvoice> invoices)
        {
            this.Invoices = invoices;
        }

        public List<ProformaInvoice> Invoices { get; set; }
    }
}

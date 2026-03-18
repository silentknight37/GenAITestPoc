using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Envelope
{
    public class ProformaInvoiceByIdEnvelop
    {
        public ProformaInvoiceByIdEnvelop(ProformaInvoice invoice)
        {
            this.Invoice = invoice;
        }

        public ProformaInvoice Invoice { get; set; }
    }
}

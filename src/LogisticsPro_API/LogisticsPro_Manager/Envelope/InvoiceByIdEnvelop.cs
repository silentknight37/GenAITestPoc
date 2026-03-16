using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Envelope
{
    public class InvoiceByIdEnvelop
    {
        public InvoiceByIdEnvelop(Invoice invoice)
        {
            this.Invoice = invoice;
        }

        public Invoice Invoice { get; set; }
    }
}

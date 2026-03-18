using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Envelope
{
    public class ProformaInvoiceReceiptsEnvelop
    {
        public ProformaInvoiceReceiptsEnvelop(List<ProformaInvoiceReceipt> receipts)
        {
            this.Receipts = receipts;
        }

        public List<ProformaInvoiceReceipt> Receipts { get; set; }
    }
}

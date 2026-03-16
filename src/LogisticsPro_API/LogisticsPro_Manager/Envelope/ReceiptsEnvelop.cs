using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Envelope
{
    public class ReceiptsEnvelop
    {
        public ReceiptsEnvelop(List<Receipt> receipts)
        {
            this.Receipts = receipts;
        }

        public List<Receipt> Receipts { get; set; }
    }
}

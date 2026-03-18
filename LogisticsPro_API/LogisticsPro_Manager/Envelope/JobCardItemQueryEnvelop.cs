using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Envelope
{
    public class JobCardItemQueryEnvelop
    {
        public JobCardItemQueryEnvelop(LineGenerateItem lineGenerateItem, List<ProformaInvoice> proformaInvoices)
        {
            this.LineGenerateItem = lineGenerateItem;
            this.ProformaInvoices = proformaInvoices;
        }

        public LineGenerateItem LineGenerateItem { get; set; }
        public List<ProformaInvoice> ProformaInvoices { get; set; }
    }
}

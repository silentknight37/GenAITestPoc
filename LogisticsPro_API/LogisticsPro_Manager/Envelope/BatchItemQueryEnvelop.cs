using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Envelope
{
    public class BatchItemQueryEnvelop
    {
        public BatchItemQueryEnvelop(List<BatchLineItem> jobCardTransportations)
        {
            this.JobCardTransportation = jobCardTransportations;
        }

        public List<BatchLineItem> JobCardTransportation { get; set; }
    }
}

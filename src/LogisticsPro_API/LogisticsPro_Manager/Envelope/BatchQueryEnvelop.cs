using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Envelope
{
    public class BatchQueryEnvelop
    {
        public BatchQueryEnvelop(List<Batch> batches)
        {
            this.Batches = batches;
        }

        public List<Batch> Batches { get; set; }
    }
}

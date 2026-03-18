using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Envelope
{
    public class JobCardTransportationQueryEnvelop
    {
        public JobCardTransportationQueryEnvelop(JobCardTransportation jobCardTransportation)
        {
            this.JobCardTransportation = jobCardTransportation;
        }

        public JobCardTransportation JobCardTransportation { get; set; }
    }
}

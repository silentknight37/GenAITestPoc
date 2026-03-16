using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Envelope
{
    public class JobCardVisaQueryEnvelop
    {
        public JobCardVisaQueryEnvelop(JobCardVisa jobCardVisa)
        {
            this.JobCardVisa = jobCardVisa;
        }

        public JobCardVisa JobCardVisa { get; set; }
    }
}

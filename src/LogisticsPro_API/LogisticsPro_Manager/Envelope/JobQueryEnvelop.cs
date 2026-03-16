using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Envelope
{
    public class JobQueryEnvelop
    {
        public JobQueryEnvelop(List<JobCard> jobCards)
        {
            this.JobCards = jobCards;
        }

        public List<JobCard> JobCards { get; set; }
    }
}

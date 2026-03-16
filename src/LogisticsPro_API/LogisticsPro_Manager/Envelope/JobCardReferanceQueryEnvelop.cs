using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Envelope
{
    public class JobCardReferanceQueryEnvelop
    {
        public JobCardReferanceQueryEnvelop(List<ReferenceData> jobCards)
        {
            this.JobCards = jobCards;
        }

        public List<ReferenceData> JobCards { get; set; }

    }
}

using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Envelope
{
    public class JobCardQueryEnvelop
    {
        public JobCardQueryEnvelop(JobCard jobCard)
        {
            this.JobCard = jobCard;
        }

        public JobCard JobCard { get; set; }
    }
}

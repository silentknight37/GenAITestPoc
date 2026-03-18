using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Envelope
{
    public class JobCardMiscellaneousQueryEnvelop
    {
        public JobCardMiscellaneousQueryEnvelop(JobCardMiscellaneous jobCardMiscellaneous)
        {
            this.JobCardMiscellaneous = jobCardMiscellaneous;
        }

        public JobCardMiscellaneous JobCardMiscellaneous { get; set; }
    }
}

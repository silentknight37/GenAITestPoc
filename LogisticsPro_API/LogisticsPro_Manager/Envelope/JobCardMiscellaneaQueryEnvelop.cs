using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Envelope
{
    public class JobCardMiscellaneaQueryEnvelop
    {
        public JobCardMiscellaneaQueryEnvelop(List<JobCardMiscellaneous> jobCardMiscellaneous, int? jobCardStatusId)
        {
            this.JobCardMiscellaneous = jobCardMiscellaneous;
            this.JobCardStatusId = jobCardStatusId;
        }

        public List<JobCardMiscellaneous> JobCardMiscellaneous { get; set; }
        public int? JobCardStatusId { get; set; }
        public decimal TotalSellAmount
        {
            get
            {
                return JobCardMiscellaneous.Any() ? JobCardMiscellaneous.Sum(i => i.TotalSellPrice) : 0;
            }
        }

        public decimal TotalCostAmount
        {
            get
            {
                return JobCardMiscellaneous.Any() ? JobCardMiscellaneous.Sum(i => i.TotalCostPrice) : 0;
            }
        }

        public decimal Profit
        {
            get
            {
                return TotalSellAmount - TotalCostAmount;
            }
        }
    }
}

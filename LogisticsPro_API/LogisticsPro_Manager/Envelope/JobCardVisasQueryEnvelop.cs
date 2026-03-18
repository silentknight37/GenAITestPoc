using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Envelope
{
    public class JobCardVisasQueryEnvelop
    {
        public JobCardVisasQueryEnvelop(List<JobCardVisa> jobCardVisas,int? jobCardStatusId)
        {
            this.JobCardVisas = jobCardVisas;
            this.JobCardStatusId = jobCardStatusId;
        }

        public List<JobCardVisa> JobCardVisas { get; set; }
        public int? JobCardStatusId { get; set; }
        public decimal TotalSellAmount
        {
            get
            {
                return JobCardVisas.Any() ? JobCardVisas.Sum(i => i.TotalSellPrice) : 0;
            }
        }

        public decimal TotalCostAmount
        {
            get
            {
                return JobCardVisas.Any() ? JobCardVisas.Sum(i => i.TotalCostPrice) : 0;
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

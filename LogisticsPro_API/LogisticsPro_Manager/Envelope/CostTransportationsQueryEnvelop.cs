using LogisticsPro_Common.DTO;
using System.Linq;

namespace LogisticsPro_Manager.Envelope
{
    public class CostTransportationsQueryEnvelop
    {
        public CostTransportationsQueryEnvelop(List<JobCardTransportation> jobCardTransportations)
        {
            this.JobCardTransportations = jobCardTransportations;
        }

        public List<JobCardTransportation> JobCardTransportations { get; set; }
        public decimal TotalSellAmount
        {
            get
            {
                return JobCardTransportations.Any() ? JobCardTransportations.Sum(i=>i.TotalSellPrice):0;
            }
        }

        public decimal TotalCostAmount
        {
            get
            {
                return JobCardTransportations.Any()?JobCardTransportations.Sum(i => i.TotalCostPrice):0;
            }
        }

        public decimal Profit
        {
            get
            {
                return TotalSellAmount- TotalCostAmount;
            }
        }
    }
}

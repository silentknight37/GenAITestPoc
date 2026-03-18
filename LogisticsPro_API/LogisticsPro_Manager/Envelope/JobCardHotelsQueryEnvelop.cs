using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Envelope
{
    public class JobCardHotelsQueryEnvelop
    {
        public JobCardHotelsQueryEnvelop(List<JobCardHotel> jobCardHotels, int? jobCardStatusId)
        {
            this.JobCardHotels = jobCardHotels;
            this.JobCardStatusId = jobCardStatusId;
        }

        public List<JobCardHotel> JobCardHotels { get; set; }
        public int? JobCardStatusId { get; set; }
        public decimal TotalSellAmount
        {
            get
            {
                return JobCardHotels.Any() ? JobCardHotels.Sum(i => i.TotalSellPrice) : 0;
            }
        }

        public decimal TotalCostAmount
        {
            get
            {
                return JobCardHotels.Any() ? JobCardHotels.Sum(i => i.TotalCostPrice) : 0;
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

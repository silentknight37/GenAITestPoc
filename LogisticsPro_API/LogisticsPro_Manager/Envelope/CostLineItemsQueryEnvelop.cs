using LogisticsPro_Common.DTO;
using System.Linq;

namespace LogisticsPro_Manager.Envelope
{
    public class CostLineItemsQueryEnvelop
    {
        public CostLineItemsQueryEnvelop(int referenceTypeId,List<JobCardTransportation> jobCardTransportations, List<JobCardHotel> jobCardHotels, List<JobCardVisa> jobCardVisas, List<JobCardMiscellaneous> jobCardMiscellaneous)
        {
            this.ReferenceTypeId = referenceTypeId;
            this.JobCardTransportations = jobCardTransportations;
            this.JobCardHotels = jobCardHotels;
            this.JobCardVisas = jobCardVisas;
            this.JobCardMiscellaneous = jobCardMiscellaneous;
        }
        public int ReferenceTypeId { get; set; }
        public List<JobCardTransportation> JobCardTransportations { get; set; }
        public List<JobCardHotel> JobCardHotels { get; set; }
        public List<JobCardVisa> JobCardVisas { get; set; }
        public List<JobCardMiscellaneous> JobCardMiscellaneous { get; set; }
        public decimal TotalSellAmount
        {
            get
            {
                switch (ReferenceTypeId)
                {
                    case 1:
                        {
                            return JobCardTransportations.Any() ? JobCardTransportations.Sum(i => i.TotalSellPrice) : 0;
                        }
                    case 2:
                        {
                            return JobCardHotels.Any() ? JobCardHotels.Sum(i => i.TotalSellPrice) : 0;
                        }
                    case 3:
                        {
                            return JobCardVisas.Any() ? JobCardVisas.Sum(i => i.TotalSellPrice) : 0;
                        }
                    case 4:
                        {
                            return JobCardMiscellaneous.Any() ? JobCardMiscellaneous.Sum(i => i.TotalSellPrice) : 0;
                        }
                    default:
                        return 0;
                }
            }
        }

        public decimal TotalCostAmount
        {
            get
            {
                switch (ReferenceTypeId)
                {
                    case 1:
                        {
                            return JobCardTransportations.Any() ? JobCardTransportations.Sum(i => i.TotalCostPrice) : 0;
                        }
                    case 2:
                        {
                            return JobCardHotels.Any() ? JobCardHotels.Sum(i => i.TotalCostPrice) : 0;
                        }
                    case 3:
                        {
                            return JobCardVisas.Any() ? JobCardVisas.Sum(i => i.TotalCostPrice) : 0;
                        }
                    case 4:
                        {
                            return JobCardMiscellaneous.Any() ? JobCardMiscellaneous.Sum(i => i.TotalCostPrice) : 0;
                        }
                    default:
                        return 0;
                }
            }
        }

        public decimal Profit
        {
            get
            {
                switch (ReferenceTypeId)
                {
                    case 1:
                        {
                            return TotalSellAmount - TotalCostAmount;
                        }
                    case 2:
                        {
                            return TotalSellAmount - TotalCostAmount;
                        }
                    case 3:
                        {
                            return TotalSellAmount - TotalCostAmount;
                        }
                    case 4:
                        {
                            return TotalSellAmount - TotalCostAmount;
                        }
                    default:
                        return 0;
                }
            }
        }
    }
}

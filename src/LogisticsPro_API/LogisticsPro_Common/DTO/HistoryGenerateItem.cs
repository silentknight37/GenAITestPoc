using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class HistoryGenerateItem
    {
        public HistoryGenerateItem() {
            JobCards = new List<JobCardHistory>();
            Transportations = new List<JobCardTransportationHistory>();
            Hotels = new List<JobCardHotelHistory>();
            Visa = new List<JobCardVisaHistory>();
            Miscellaneous = new List<JobCardMiscellaneousHistory>();
        }
        public List<JobCardHistory> JobCards { get; set; }
        public List<JobCardTransportationHistory> Transportations { get; set; }
        public List<JobCardHotelHistory> Hotels { get; set; }
        public List<JobCardVisaHistory> Visa { get; set; }
        public List<JobCardMiscellaneousHistory> Miscellaneous { get; set; }
    }
}

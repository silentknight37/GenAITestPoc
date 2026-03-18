using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class LineGenerateItem
    {
        public LineGenerateItem() {
            Transportations = new List<JobCardTransportation>();
            Hotels = new List<JobCardHotel>();
            Visa = new List<JobCardVisa>();
            Miscellaneous = new List<JobCardMiscellaneous>();
        }
        public List<JobCardTransportation> Transportations { get; set; }
        public List<JobCardHotel> Hotels { get; set; }
        public List<JobCardVisa> Visa { get; set; }
        public List<JobCardMiscellaneous> Miscellaneous { get; set; }
    }
}

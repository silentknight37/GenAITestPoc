using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_API.Request
{
    public class SavePaymentVoucherRequest
    {
        public List<int> TransportationIds { get; set; }
        public List<int> HotelIds { get; set; }
        public List<int> VisaIds { get; set; }
        public List<int> MiscellaneousIds { get; set; }
        public int VendorId { get; set; }
        public string PaymentVoucherDate { get; set; }
        public decimal PaymentVoucherAmount { get; set; }
        public string Invoice { get; set; }
        public string Remarks { get; set; }
    }
}

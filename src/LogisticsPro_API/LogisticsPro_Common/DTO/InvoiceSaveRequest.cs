using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class InvoiceSaveRequest
    {
        public int UserId { get; set; }
        public List<int> TransportationIds { get; set; }
        public List<int> HotelIds { get; set; }
        public List<int> VisaIds { get; set; }
        public List<int> MiscellaneousIds { get; set; }
        public int CustomerId { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? InvoiceDueDate { get; set; }
        public decimal InvoiceAmount { get; set; }
        public string Remarks { get; set; }
        public string TransportDescription { get; set; }
        public string HotelDescription { get; set; }
        public string VisaDescription { get; set; }
        public string MiscellaneousDescription { get; set; }
        public List<int> PerformaInvoiceIds { get; set; }
    }
}

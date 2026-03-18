using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_API.Request
{
    public class SaveProformaInvoiceRequest
    {
        public int CustomerId { get; set; }
        public string InvoiceDate { get; set; }
        public string InvoiceDueDate { get; set; }
        public decimal InvoiceAmount { get; set; }
        public int JobCardId { get; set; }
        public string Description { get; set; }
    }
}

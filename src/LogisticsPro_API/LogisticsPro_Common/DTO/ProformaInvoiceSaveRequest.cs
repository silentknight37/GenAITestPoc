using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class ProformaInvoiceSaveRequest
    {
        public int UserId { get; set; }
        public int CustomerId { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? InvoiceDueDate { get; set; }
        public decimal InvoiceAmount { get; set; }
        public int JobCardId { get; set; }
        public string Description { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class JobCardFinanceRreceipt
    {
        public int Id { get; set; }
        public int? JobCardId { get; set; }
        public string ReceiptCode { get; set; }
        public string InvoiceNo { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public string Remarks { get; set; }
    }
}

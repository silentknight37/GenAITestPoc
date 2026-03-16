using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class ProformaInvoiceReceiptSaveRequest
    {
        public int UserId { get; set; }
        public int Id { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public decimal Amount { get; set; }
        public int? PaymentMethod { get; set; }
        public int? ProformaInvoiceId { get; set; }
        public int? JobCardId { get; set; }
        public string Remark { get; set; }
    }
}

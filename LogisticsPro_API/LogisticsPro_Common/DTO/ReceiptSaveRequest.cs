using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class ReceiptSaveRequest
    {
        public ReceiptSaveRequest() {
            UpdateRecords=new List<UpdateRecords>();
        }
        public int UserId { get; set; }
        public int Id { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public decimal Amount { get; set; }
        public int? PaymentMethod { get; set; }
        public int? InvoiceId { get; set; }
        public string Remark { get; set; }
        public List<UpdateRecords> UpdateRecords { get; set; }
    }
    public class UpdateRecords
    {
        public int Id { get; set; }
        public string ServiceType { get; set; }
        public decimal AllocatedAmount { get; set; }

    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_API.Request
{
    public class SaveReceiptRequest
    {
        public SaveReceiptRequest()
        {
            UpdateRecords=new List<UpdateRecords>();    
        }
        public int Id { get; set; }
        public string ReceiptDate { get; set; }
        public decimal Amount { get; set; }
        public int PaymentMethod { get; set; }
        public int InvoiceId { get; set; }
        public string Remark { get; set; }
        public List<UpdateRecords> UpdateRecords { get; set; }
    }
    public class UpdateRecords
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public decimal AllocatedAmount { get; set; }

    }
}

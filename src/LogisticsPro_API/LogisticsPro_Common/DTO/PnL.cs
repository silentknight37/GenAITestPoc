using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class PnL
    {
        public int JobCardId { get; set; }
        public string JobCardNo { get; set; }
        public string JobCardDescription { get; set; }
        public string CustomerName { get; set; }
        public decimal? InvoiceAmount { get; set; }
        public decimal? ReceiptAmount { get; set; }
        public decimal? PaymentVoucherAmount { get; set; }
        public decimal ProfitLost
        {
            get
            {
                return (ReceiptAmount==null?0: ReceiptAmount.Value) - (PaymentVoucherAmount==null?0: PaymentVoucherAmount.Value);
            }
        }
        public string Description
        {
            get
            {
                return $"{JobCardNo} - {JobCardDescription}";
            }
        }
    }
}

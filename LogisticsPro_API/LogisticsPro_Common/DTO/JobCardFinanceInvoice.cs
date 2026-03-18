using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class JobCardFinanceInvoice
    {
        public int Id { get; set; }
        public int? JobCardId { get; set; }
        public string InvoiceCode { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? InvoiceDueDate { get; set; }
        public decimal? Amount { get; set; }
        public decimal? LineItemSellAmount { get; set; }
        public decimal? LineItemTaxAmount { get; set; }
        public decimal? LineItemTotalAmount { get; set; }
        public decimal? LineItemAmount
        {
            get
            {
                var baseAmount = LineItemSellAmount.HasValue ? LineItemSellAmount.Value : 0;
                var taxAmount = LineItemTaxAmount.HasValue ? LineItemTaxAmount.Value : 0;

                return baseAmount + taxAmount;
            }
        }
        public string Remarks { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class ProformaInvoice
    {
        public ProformaInvoice() {
            ProformaInvoiceReceipts=new List<ProformaInvoiceReceipt>();
        }
        public int Id { get; set; }
        public string InvoiceCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddressLine1 { get; set; }
        public string CustomerAddressLine2 { get; set; }
        public string StatusName { get; set; }
        public string Description { get; set; }
        public string? City { get; set; }
        public int? CountryCode { get; set; }
        public int StatusId { get; set; }
        public int? CustomerId { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? InvoiceDueDate { get; set; }
        public decimal? InvoiceAmount { get; set; }
        public int JobCardId { get; set; }
        public string JobCardNo { get; set; }
        public List<ProformaInvoiceReceipt> ProformaInvoiceReceipts { get; set; }

        public decimal TotalReciptAmount
        {
            get
            {
                var totalReceiptAmount= ProformaInvoiceReceipts.Select(i=>i.Amount).Sum();

                return totalReceiptAmount.Value;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class Receipt
    {
        public int Id { get; set; }
        public int? InvoiceId { get; set; }
        public string InvoiceCode { get; set; }
        public string CustomerName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string ReceiptCode { get; set; }
        public int? CountryCode { get; set; }
        public string City { get; set; }
        public int? PaymentMethodId { get; set; }
        public string PaymentMethod { get; set; }
        public string Remark { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal? Amount { get; set; }
        public string Status { get; set; }
    }
}

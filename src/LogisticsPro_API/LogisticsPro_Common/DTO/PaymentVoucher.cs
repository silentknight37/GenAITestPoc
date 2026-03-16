using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class PaymentVoucher
    {
        public PaymentVoucher()
        {
            PaymentVoucherGenerateItem = new LineGenerateItem();
        }
        public int Id { get; set; }
        public string PaymentVoucherCode { get; set; }
        public int? VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorAddressLine1 { get; set; }
        public string VendorAddressLine2 { get; set; }
        public string VendorBankCode { get; set; }
        public string VendorBankName { get; set; }
        public DateTime? PaymentVoucherDate { get; set; }
        public decimal? PaymentVoucherAmount { get; set; }
        public string InvoiceNo { get; set; }
        public string Remark { get; set; }
        public LineGenerateItem PaymentVoucherGenerateItem { get; set; }
    }
}

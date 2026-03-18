using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class JobCardFinancePaymentVoucher
    {
        public int Id { get; set; }
        public int? JobCardId { get; set; }
        public string VoucherCode { get; set; }
        public string VendorName { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? VoucherDate { get; set; }
        public string Remarks { get; set; }
    }
}

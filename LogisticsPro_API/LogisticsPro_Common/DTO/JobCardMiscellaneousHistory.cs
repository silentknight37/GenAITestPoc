using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class JobCardMiscellaneousHistory
    {
        public int Id { get; set; }
        public string MiscellaneousCode { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public int? JobCardId { get; set; }
        public string JobCardNo { get; set; }
        public string PaxName { get; set; }
        public string PaxNumber { get; set; }
        public string Description { get; set; }
        public DateTime? MisDate { get; set; }
        public bool? IsVatIncludedCost { get; set; }
        public bool? IsVatIncludedSell { get; set; }
        public bool? IsInvoiced { get; set; }
        public bool? IsPaymentVouchered { get; set; }
        public bool? IsBatched { get; set; }
        public bool? IsFinance { get; set; }
        public decimal? CostBaseAmount { get; set; }
        public decimal? CostTaxAmount { get; set; }
        public decimal? SellBaseAmount { get; set; }
        public decimal? SellTaxAmount { get; set; }
        public decimal? ReceiptBalanceAmount { get; set; }
        public decimal? TotalReceiptAmount { get; set; }
        public string Remarks { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CreatedByName { get; set; }
        public string? UpdatedByName { get; set; }
        public string AuditAction { get; set; }
        public DateTime? AuditDate { get; set; }
        public decimal TotalCostPrice
        {
            get
            {
                return CostBaseAmount.HasValue && CostTaxAmount.HasValue ? CostBaseAmount.Value + CostTaxAmount.Value : 0;
            }
        }
        public decimal TotalSellPrice
        {
            get
            {
                return SellBaseAmount.HasValue && SellTaxAmount.HasValue ? SellBaseAmount.Value + SellTaxAmount.Value : 0;
            }
        }
    }
}

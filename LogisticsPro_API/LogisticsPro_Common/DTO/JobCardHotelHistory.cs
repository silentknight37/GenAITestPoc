using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class JobCardHotelHistory
    {
        public int Id { get; set; }
        public string HotelCode { get; set; }
        public int? JobCardId { get; set; }
        public string JobCardNo { get; set; }
        public string PaxName { get; set; }
        public int? Adults { get; set; }
        public int? Children { get; set; }
        public int? Infants { get; set; }
        public string HotelName { get; set; }
        public string VenderName { get; set; }
        public int VenderId { get; set; }
        public string HotelConfirmation { get; set; }
        public string RoomType { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public bool? IsVatIncludedCost { get; set; }
        public bool? IsVatIncludedSell { get; set; }
        public bool? IsInvoiced { get; set; }
        public bool? IsPaymentVouchered { get; set; }
        public bool? IsBatched { get; set; }
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
        public int TotalPax
        {
            get
            {
                return Adults.HasValue && Children.HasValue && Infants.HasValue ? Adults.Value + Children.Value + Infants.Value : 0;
            }
        }
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

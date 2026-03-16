using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class ReportItem
    {
        public int Id { get; set; }
        public string BookingRef { get; set; }
        public string JobCardNo { get; set; }
        public string CustomerRef { get; set; }
        public string PaxName { get; set; }
        public int? Adults { get; set; }
        public int? Children { get; set; }
        public int? Infants { get; set; }
        public string VehicleType { get; set; }
        public string PickupLocation { get; set; }
        public DateTime? PickupTime { get; set; }
        public string DropoffLocation { get; set; }
        public string FlightNo { get; set; }
        public DateTime? FlightTime { get; set; }
        public bool? IsVatIncludedCost { get; set; }
        public bool? IsVatIncludedSell { get; set; }
        public decimal? CostBaseAmount { get; set; }
        public decimal? CostTaxAmount { get; set; }
        public decimal? SellBaseAmount { get; set; }
        public decimal? SellTaxAmount { get; set; }
        public string Remarks { get; set; }
        public string CustomerName { get; set; }
        public string SuplierName { get; set; }
        public string BatchNo { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
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

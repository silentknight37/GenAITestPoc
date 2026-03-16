using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class InvoiceLineItem
    {
        public int Id { get; set; }
        public string BookingRef { get; set; }
        public string ContextTypeName { get; set; }
        public string ContextTypeId { get; set; }
        public int? JobCardId { get; set; }
        public string JobCardNo { get; set; }
        public string JobCardDescription { get; set; }
        public string CustomerName { get; set; }
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
        public decimal? Parking { get; set; }
        public decimal? ParkingTaxAmount { get; set; }
        public decimal? Water { get; set; }
        public decimal? WaterTaxAmount { get; set; }
        public decimal? Extras { get; set; }
        public decimal? ExtrasTaxAmount { get; set; }
        public decimal? ParkingSell { get; set; }
        public decimal? ParkingTaxAmountSell { get; set; }
        public decimal? WaterSell { get; set; }
        public decimal? WaterTaxAmountSell { get; set; }
        public decimal? ExtrasSell { get; set; }
        public decimal? ExtrasTaxAmountSell { get; set; }
        public decimal? ReceiptBalanceAmount { get; set; }
        public decimal? TotalReceiptAmount { get; set; }
        public string Remarks { get; set; }
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
                var costBase = CostBaseAmount.HasValue ? CostBaseAmount.Value : 0;
                var costTax = CostTaxAmount.HasValue ? CostTaxAmount.Value : 0;

                var extra = Extras.HasValue ? Extras.Value : 0;
                var extraTaxAmount = ExtrasTaxAmount.HasValue ? ExtrasTaxAmount.Value : 0;
                var parking = Parking.HasValue ? Parking.Value : 0;
                var parkingTaxAmount = ParkingTaxAmount.HasValue ? ParkingTaxAmount.Value : 0;
                var water = Water.HasValue ? Water.Value : 0;
                var waterTaxAmount = WaterTaxAmount.HasValue ? WaterTaxAmount.Value : 0;

                return costBase + costTax + extra + parking + water + extraTaxAmount+ parkingTaxAmount+ waterTaxAmount;
            }
        }

        public decimal TotalCostTaxAmount
        {
            get
            {
                var costtax = CostTaxAmount.HasValue ? CostTaxAmount.Value : 0;
                var extraTaxAmount = ExtrasTaxAmount.HasValue ? ExtrasTaxAmount.Value : 0;
                var parkingTaxAmount = ParkingTaxAmount.HasValue ? ParkingTaxAmount.Value : 0;
                var waterTaxAmount = WaterTaxAmount.HasValue ? WaterTaxAmount.Value : 0;

                return costtax + extraTaxAmount+ parkingTaxAmount+ waterTaxAmount;
            }
        }

        public decimal TotalSellPrice
        {
            get
            {
                var costBase = SellBaseAmount.HasValue ? SellBaseAmount.Value : 0;
                var costTax = SellTaxAmount.HasValue ? SellTaxAmount.Value : 0;

                var extra = ExtrasSell.HasValue ? ExtrasSell.Value : 0;
                var extraTaxAmount = ExtrasTaxAmountSell.HasValue ? ExtrasTaxAmountSell.Value : 0;
                var parking = ParkingSell.HasValue ? ParkingSell.Value : 0;
                var parkingTaxAmount = ParkingTaxAmountSell.HasValue ? ParkingTaxAmountSell.Value : 0;
                var water = WaterSell.HasValue ? WaterSell.Value : 0;
                var waterTaxAmount = WaterTaxAmountSell.HasValue ? WaterTaxAmountSell.Value : 0;

                return costBase + costTax + extra + parking + water + extraTaxAmount+ parkingTaxAmount+ waterTaxAmount;
            }
        }

        public decimal TotalSellTaxAmount
        {
            get
            {
                var costtax = SellTaxAmount.HasValue ? SellTaxAmount.Value : 0;
                var extraTaxAmount = ExtrasTaxAmountSell.HasValue ? ExtrasTaxAmountSell.Value : 0;
                var parkingTaxAmount = ParkingTaxAmountSell.HasValue ? ParkingTaxAmountSell.Value : 0;
                var waterTaxAmount = WaterTaxAmountSell.HasValue ? WaterTaxAmountSell.Value : 0;

                return costtax + extraTaxAmount+ parkingTaxAmount+ waterTaxAmount;
            }
        }

        public string JobCardTitle
        {
            get
            {
                return $"{JobCardNo} - {JobCardDescription}";
            }
        }

    }
}

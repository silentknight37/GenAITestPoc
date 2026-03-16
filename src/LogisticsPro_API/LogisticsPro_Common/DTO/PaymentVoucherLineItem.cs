using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class PaymentVoucherLineItem
    {
        public int Id { get; set; }
        public string BatchNo { get; set; }
        public string JobCardNo { get; set; }
        public string BookingRef { get; set; }
        public string CustomerName { get; set; }
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
        public decimal? CostBaseAmount { get; set; }
        public decimal? CostTaxAmount { get; set; }
        public decimal? Parking { get; set; }
        public decimal? ParkingTaxAmount { get; set; }
        public decimal? Water { get; set; }
        public decimal? WaterTaxAmount { get; set; }
        public decimal? Extras { get; set; }
        public decimal? ExtrasTaxAmount { get; set; }
        public string Remarks { get; set; }
        public int TotalPax
        {
            get
            {
                return Adults.HasValue && Children.HasValue && Infants.HasValue ? Adults.Value + Children.Value + Infants.Value : 0;
            }
        }
        public string PickupTimeStn 
        {
            get
            {
                return PickupTime==null?"":PickupTime.Value.ToString("MM/dd/yyyy H:mm");
            }
        }
        public string FlightTimeStn
        {
            get
            {
                return FlightTime == null ? "" : FlightTime.Value.ToString("MM/dd/yyyy H:mm");
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

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_API.Request
{
    public class SaveJobCardTransportationRequest
    {
        public int Id { get; set; }
        public int JobCardId { get; set; }
        public string CustomerRef { get; set; }
        public string PaxName { get; set; }
        public int? Adults { get; set; }
        public int? Children { get; set; }
        public int? Infants { get; set; }
        public string VehicleType { get; set; }
        public string PickupLocation { get; set; }
        public string? PickupTime { get; set; }
        public string DropoffLocation { get; set; }
        public string FlightNo { get; set; }
        public string? FlightTime { get; set; }
        public bool IsVatIncludedCost { get; set; }
        public bool IsVatIncludedSell { get; set; }
        public decimal CostBaseAmount { get; set; }
        public decimal CostTaxAmount { get; set; }
        public decimal SellBaseAmount { get; set; }
        public decimal SellTaxAmount { get; set; }
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
        public string Remarks { get; set; }
    }
}

using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Command
{
    public class SaveJobCardTransportationCommand : IRequest<RequestSaveEnvelop>
    {
        public SaveJobCardTransportationCommand(int id, int jobCardId, string customerRef, string paxName, int? adults, int? children, int? infants,
            string vehicleType, string pickupLocation, string? pickupTime, string dropoffLocation, string flightNo, string? flightTime, bool isVatIncludedCost, bool isVatIncludedSell, decimal costBaseAmount, decimal costTaxAmount, decimal sellBaseAmount, decimal sellTaxAmount, decimal? parking, decimal? parkingTaxAmount, decimal? water, decimal? waterTaxAmount, decimal? extras, decimal? extrasTaxAmount, decimal? parkingSell, decimal? parkingTaxAmountSell, decimal? waterSell, decimal? waterTaxAmountSell, decimal? extrasSell, decimal? extrasTaxAmountSell, string remarks,int userId) 
        {
            this.Id = id;
            this.JobCardId = jobCardId;
            this.CustomerRef = customerRef;
            this.PaxName = paxName;
            this.Adults = adults;
            this.Children = children;
            this.Infants = infants;
            this.VehicleType = vehicleType;
            this.PickupLocation = pickupLocation;
            this.PickupTimeStn = pickupTime;
            this.DropoffLocation = dropoffLocation;
            this.FlightNo = flightNo;
            this.FlightTimeStn = flightTime;
            this.IsVatIncludedCost = isVatIncludedCost;
            this.IsVatIncludedSell = isVatIncludedSell;
            this.CostBaseAmount = costBaseAmount;
            this.CostTaxAmount = costTaxAmount;
            this.SellBaseAmount = sellBaseAmount;
            this.SellTaxAmount = sellTaxAmount;
            this.Parking = parking;
            this.ParkingTaxAmount= parkingTaxAmount;
            this.Water = water;
            this.WaterTaxAmount = waterTaxAmount;
            this.Extras = extras;
            this.ExtrasTaxAmount = extrasTaxAmount;
            this.ParkingSell = parkingSell;
            this.ParkingTaxAmountSell= parkingTaxAmountSell;
            this.WaterSell = waterSell;
            this.WaterTaxAmountSell= waterTaxAmountSell;
            this.ExtrasSell = extrasSell;
            this.ExtrasTaxAmountSell = extrasTaxAmountSell;
            this.Remarks = remarks;
            this.UserId = userId;
        }

        public int UserId { get; set; }
        public int Id { get; set; }
        public int JobCardId { get; set; }
        public string CustomerRef { get; set; }
        public string PaxName { get; set; }
        public int? Adults { get; set; }
        public int? Children { get; set; }
        public int? Infants { get; set; }
        public string VehicleType { get; set; }
        public string PickupLocation { get; set; }
        public string? PickupTimeStn { get; set; }
        public DateTime? PickupTime
        {
            get
            {
                return PickupTimeStn != null ? DateTimeOffset.Parse(PickupTimeStn).LocalDateTime : null;
            }
        }
        public string DropoffLocation { get; set; }
        public string FlightNo { get; set; }
        public DateTime? FlightTime
        {
            get
            {
                return FlightTimeStn != null ? DateTimeOffset.Parse(FlightTimeStn).LocalDateTime : null;
            }
        }
        public string? FlightTimeStn { get; set; }
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

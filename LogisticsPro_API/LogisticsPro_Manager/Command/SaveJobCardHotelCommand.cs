using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Command
{
    public class SaveJobCardHotelCommand : IRequest<RequestSaveEnvelop>
    {
        public SaveJobCardHotelCommand(int id,int jobCardId, string paxName,int? adults,int? children,int? infants,
            int vendorId,string hotelName, string? checkIn, string? checkOut,bool isVatIncludedCost, bool isVatIncludedSell, decimal costBaseAmount,decimal costTaxAmount,decimal sellBaseAmount,decimal sellTaxAmount, string remarks,string hotelConfirmation,string roomType,string hotelAddress1,string hotelAddress2,int userId) 
        {
            this.Id = id;
            this.JobCardId = jobCardId;
            this.PaxName = paxName;
            this.Adults = adults;
            this.Children = children;
            this.Infants = infants;
            this.VendorId = vendorId;
            this.HotelName = hotelName;
            this.CheckInStn = checkIn;
            this.CheckOutStn = checkOut;
            this.IsVatIncludedCost = isVatIncludedCost;
            this.IsVatIncludedSell = isVatIncludedSell;
            this.CostBaseAmount = costBaseAmount;
            this.CostTaxAmount = costTaxAmount;
            this.SellBaseAmount = sellBaseAmount;
            this.SellTaxAmount = sellTaxAmount;
            this.Remarks = remarks;
            this.HotelConfirmation = hotelConfirmation;
            this.RoomType = roomType;
            this.HotelAddress1= hotelAddress1;
            this.HotelAddress2= hotelAddress2;
            this.UserId = userId;
        }

        public int UserId { get; set; }
        public int Id { get; set; }
        public string HotelCode { get; set; }
        public int JobCardId { get; set; }
        public string HotelName { get; set; }
        public string PaxName { get; set; }
        public int? Adults { get; set; }
        public int? Children { get; set; }
        public int? Infants { get; set; }
        public int VendorId { get; set; }
        public string HotelConfirmation { get; set; }
        public string RoomType { get; set; }
        public DateTime? CheckIn
        {
            get
            {
                return CheckInStn != null ? DateTimeOffset.Parse(CheckInStn).LocalDateTime : null;
            }
        }
        public string? CheckInStn { get; set; }
        public DateTime? CheckOut
        {
            get
            {
                return CheckOutStn != null ? DateTimeOffset.Parse(CheckOutStn).LocalDateTime : null;
            }
        }
        public string? CheckOutStn { get; set; }
        public bool IsVatIncludedCost { get; set; }
        public bool IsVatIncludedSell { get; set; }
        public decimal CostBaseAmount { get; set; }
        public decimal CostTaxAmount { get; set; }
        public decimal SellBaseAmount { get; set; }
        public decimal SellTaxAmount { get; set; }
        public string Remarks { get; set; }
        public string HotelAddress1 { get; set; }
        public string HotelAddress2 { get; set; }
    }
}

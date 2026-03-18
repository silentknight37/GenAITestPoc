using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_API.Request
{
    public class SaveJobCardHotelRequest
    {
        public int Id { get; set; }
        public int JobCardId { get; set; }
        public string PaxName { get; set; }
        public int? Adults { get; set; }
        public int? Children { get; set; }
        public int? Infants { get; set; }
        public int VendorId { get; set; }
        public string HotelName { get; set; }
        public string HotelAddress1 { get; set; }
        public string HotelAddress2 { get; set; }
        public string HotelConfirmation { get; set; }
        public string RoomType { get; set; }
        public string? CheckIn { get; set; }
        public string? CheckOut { get; set; }
        public bool IsVatIncludedCost { get; set; }
        public bool IsVatIncludedSell { get; set; }
        public decimal CostBaseAmount { get; set; }
        public decimal CostTaxAmount { get; set; }
        public decimal SellBaseAmount { get; set; }
        public decimal SellTaxAmount { get; set; }
        public string Remarks { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class JobCardHotel
    {
        public int Id { get; set; }
        public string HotelCode { get; set; }
        public int? JobCardId { get; set; }
        public int? JobCardStatusId { get; set; }
        public string JobCardDescription { get; set; }
        public string PaxName { get; set; }
        public int? Adults { get; set; }
        public int? Children { get; set; }
        public int? Infants { get; set; }
        public string HotelName { get; set; }
        public string HotelAddress1 { get; set; }
        public string HotelAddress2 { get; set; }
        public string VenderName { get; set; }
        public string JobCardNo { get; set; }
        public string VenderAddress1 { get; set; }
        public string VenderAddress2 { get; set; }
        public string CountryCode { get; set; }
        public string City { get; set; }
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
        public string JobCardTitle
        {
            get
            {
                return $"{JobCardNo} - {JobCardDescription}";
            }
        }

        public int Nights
        {
            get
            {
                if(!CheckIn.HasValue || !CheckOut.HasValue)
                {
                    return 0;
                }

                //var totalDays = (int)(CheckOut.Value - CheckIn.Value).TotalDays;

                TimeSpan timeDifference = CheckOut.Value - CheckIn.Value;

                int numberOfNights = (int)Math.Ceiling(timeDifference.TotalDays);

                return numberOfNights;// totalDays > 1 ? totalDays : 0;
            }
        }
    }
}

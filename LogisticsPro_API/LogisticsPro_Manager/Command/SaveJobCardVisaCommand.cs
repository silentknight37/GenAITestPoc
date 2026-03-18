using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Command
{
    public class SaveJobCardVisaCommand : IRequest<RequestSaveEnvelop>
    {
        public SaveJobCardVisaCommand(int id,int jobCardId, string paxName,
            string passportNo, int visaTypeId,int vendorId, bool isVatIncludedCost, bool isVatIncludedSell, decimal costBaseAmount,decimal costTaxAmount,decimal sellBaseAmount,decimal sellTaxAmount, string remarks,int? nationality,int userId) 
        {
            this.Id = id;
            this.JobCardId = jobCardId;
            this.VendorId= vendorId;
            this.PaxName = paxName;
            this.PassportNo = passportNo;
            this.VisaTypeId = visaTypeId;
            this.IsVatIncludedCost = isVatIncludedCost;
            this.IsVatIncludedSell = isVatIncludedSell;
            this.CostBaseAmount = costBaseAmount;
            this.CostTaxAmount = costTaxAmount;
            this.SellBaseAmount = sellBaseAmount;
            this.SellTaxAmount = sellTaxAmount;
            this.Remarks = remarks;
            this.Nationality = nationality;
            this.UserId = userId;
        }

        public int UserId { get; set; }
        public int Id { get; set; }
        public int JobCardId { get; set; }
        public int VendorId { get; set; }
        public string PaxName { get; set; }
        public string PassportNo { get; set; }
        public int VisaTypeId { get; set; }
        public bool IsVatIncludedCost { get; set; }
        public bool IsVatIncludedSell { get; set; }
        public decimal CostBaseAmount { get; set; }
        public decimal CostTaxAmount { get; set; }
        public decimal SellBaseAmount { get; set; }
        public decimal SellTaxAmount { get; set; }
        public string Remarks { get; set; }
        public int? Nationality { get; set; }
    }
}

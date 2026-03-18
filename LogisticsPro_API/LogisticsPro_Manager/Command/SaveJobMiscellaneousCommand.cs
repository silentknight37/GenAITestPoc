using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Command
{
    public class SaveJobMiscellaneousCommand : IRequest<RequestSaveEnvelop>
    {
        public SaveJobMiscellaneousCommand(int id,int jobCardId,int vendorId, string paxName, string paxNumber,
            string description, string remarks, string? misDate, bool isVatIncludedCost, bool isVatIncludedSell, decimal costBaseAmount,decimal costTaxAmount,decimal sellBaseAmount,decimal sellTaxAmount,bool isFinance,int userId) 
        {
            this.Id = id;
            this.JobCardId = jobCardId;
            this.VendorId= vendorId;
            this.PaxName = paxName;
            this.PaxNumber = paxNumber;
            this.Description = description;
            this.Remarks = remarks;
            this.MisDateStn = misDate;
            this.IsVatIncludedCost = isVatIncludedCost;
            this.IsVatIncludedSell = isVatIncludedSell;
            this.CostBaseAmount = costBaseAmount;
            this.CostTaxAmount = costTaxAmount;
            this.SellBaseAmount = sellBaseAmount;
            this.SellTaxAmount = sellTaxAmount;
            this.IsFinance = isFinance;
            this.UserId = userId;
        }

        public int UserId { get; set; }
        public int Id { get; set; }
        public int VendorId { get; set; }
        public int JobCardId { get; set; }
        public string PaxName { get; set; }
        public string PaxNumber { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public DateTime? MisDate
        {
            get
            {
                return MisDateStn != null ? DateTimeOffset.Parse(MisDateStn).LocalDateTime : null;
            }
        }
        public string? MisDateStn { get; set; }
        public bool IsVatIncludedCost { get; set; }
        public bool IsVatIncludedSell { get; set; }
        public decimal CostBaseAmount { get; set; }
        public decimal CostTaxAmount { get; set; }
        public decimal SellBaseAmount { get; set; }
        public decimal SellTaxAmount { get; set; }
        public bool IsFinance { get; set; }
    }
}

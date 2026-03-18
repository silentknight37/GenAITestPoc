using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Command
{
    public class UpdateCostCommand : IRequest<RequestSaveEnvelop>
    {
        public UpdateCostCommand(int id,bool isVatIncludedCost,bool isVatIncludedSell,decimal? costBaseAmount,decimal? costTaxAmount,
            decimal? sellBaseAmount,decimal? sellTaxAmount,decimal? parking,decimal? water,decimal? extras, decimal? extrasTaxAmount, decimal? parkingSell, decimal? waterSell, decimal? extrasSell, decimal? extrasTaxAmountSell,int userId) 
        {
            this.Id = id;
            this.IsVatIncludedCost = isVatIncludedCost;
            this.IsVatIncludedSell = isVatIncludedSell;
            this.CostBaseAmount = costBaseAmount;
            this.CostTaxAmount = costTaxAmount;
            this.SellBaseAmount = sellBaseAmount;
            this.SellTaxAmount = sellTaxAmount;
            this.Parking = parking;
            this.Water = water;
            this.Extras = extras;
            this.ExtrasTaxAmount = extrasTaxAmount;
            this.ParkingSell = parkingSell;
            this.WaterSell = waterSell;
            this.ExtrasSell = extrasSell;
            this.ExtrasTaxAmountSell = extrasTaxAmountSell;
            UserId = userId;
        }

        public int Id { get; set; }
        public bool IsVatIncludedCost { get; set; }
        public bool IsVatIncludedSell { get; set; }
        public decimal? CostBaseAmount { get; set; }
        public decimal? CostTaxAmount { get; set; }
        public decimal? SellBaseAmount { get; set; }
        public decimal? SellTaxAmount { get; set; }
        public decimal? Parking { get; set; }
        public decimal? Water { get; set; }
        public decimal? Extras { get; set; }
        public decimal? ExtrasTaxAmount { get; set; }
        public decimal? ParkingSell { get; set; }
        public decimal? WaterSell { get; set; }
        public decimal? ExtrasSell { get; set; }
        public decimal? ExtrasTaxAmountSell { get; set; }
        public int UserId { get; }
    }
}

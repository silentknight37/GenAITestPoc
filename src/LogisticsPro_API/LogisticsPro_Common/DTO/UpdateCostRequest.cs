using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class UpdateCostRequest
    {
        public int UserId { get; set; }
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
    }
}

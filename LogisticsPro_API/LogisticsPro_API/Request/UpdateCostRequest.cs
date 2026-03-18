using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_API.Request
{
    public class CostUpdateRequest
    {
        public List<CostRequest> UpdatedRecords { get; set; }
    }
    public class CostRequest
    {
        public int Id { get; set; }
        public bool IsVatIncludedCost { get; set; }
        public bool IsVatIncludedSell { get; set; }
        public decimal? CostBaseAmount { get; set; }
        public decimal? CostTaxAmount { get; set; }
        public decimal? SellBaseAmount { get; set; }
        public decimal? SellTaxAmount { get; set; }
        public decimal? parkingSell { get; set; }
        public decimal? waterSell { get; set; }
        public decimal? extrasSell { get; set; }
        public decimal? extrasTaxAmountSell { get; set; }
        public decimal? Parking { get; set; }
        public decimal? Water { get; set; }
        public decimal? Extras { get; set; }
        public decimal? ExtrasTaxAmount { get; set; }
    }
}

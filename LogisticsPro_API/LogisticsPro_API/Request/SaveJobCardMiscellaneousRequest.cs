using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_API.Request
{
    public class SaveJobCardMiscellaneousRequest
    {
        public int Id { get; set; }
        public int JobCardId { get; set; }
        public int VendorId { get; set; }
        public string PaxName { get; set; }
        public string PaxNumber { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public string? MisDate { get; set; }
        public bool IsVatIncludedCost { get; set; }
        public bool IsVatIncludedSell { get; set; }
        public decimal CostBaseAmount { get; set; }
        public decimal CostTaxAmount { get; set; }
        public decimal SellBaseAmount { get; set; }
        public decimal SellTaxAmount { get; set; }
        public bool IsFinance { get; set; }
    }
}

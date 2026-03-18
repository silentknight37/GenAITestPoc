using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class JobCardHistory
    {
        public int Id { get; set; }
        public string JobCardCode { get; set; }
        public string JobCardDescription { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerRef { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string? UpdatedByName { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string AuditAction { get; set; }
        public DateTime? AuditDate { get; set; }
    }
}

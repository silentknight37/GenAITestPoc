using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class JobCard
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
        public int? StatusId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string HeaderTitle
        {
            get
            {
                return $"{JobCardCode} - {JobCardDescription}";
            }
        }
    }
}

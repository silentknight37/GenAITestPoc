using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class BatchItem
    {
        public int Id { get; set; }
        public int? BatchId { get; set; }
        public int? ContextType { get; set; }
        public int? ContextId { get; set; }
        public string ContextName { get; set; }
        public string Code { get; set; }
        public DateTime? Date { get; set; }
        public string Pax { get; set; }
    }
}

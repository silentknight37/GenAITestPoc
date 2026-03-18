using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class Event
    {
        public int Id { get; set; }
        public string? EventName { get; set; }
        public DateTime? EventFromDate { get; set; }
        public DateTime? EventToDate { get; set; }
        public int? EventTypeId { get; set; }
        public int? Customerid { get; set; }
        public string? CustomerName { get; set; }
        public string? EventType { get; set; }
        public string? Remark { get; set; }
    }
}

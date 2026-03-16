using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class EventSaveRequest
    {
        public int UserId { get; set; }
        public int Id { get; set; }
        public string? EventName { get; set; }
        public DateTime? EventFromDate { get; set; }
        public DateTime? EventToDate { get; set; }
        public int? EventTypeId { get; set; }
        public int? CustomerId { get; set; }
        public string? Remark { get; set; }
    }
}

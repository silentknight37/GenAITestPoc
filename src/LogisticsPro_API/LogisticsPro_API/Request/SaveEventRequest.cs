using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_API.Request
{
    public class SaveEventRequest
    {
        public int Id { get; set; }
        public string? EventName { get; set; }
        public string EventFromDate { get; set; }
        public string EventToDate { get; set; }
        public int? EventTypeId { get; set; }
        public int? CustomerId { get; set; }
        public string? Remark { get; set; }
    }
}

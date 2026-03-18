using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class BatchItemSaveRequest
    {
        public int UserId { get; set; }
        public List<int> Ids { get; set; }
        public int VendorId { get; set; }
        public DateTime? BatchDate { get; set; }
    }
}

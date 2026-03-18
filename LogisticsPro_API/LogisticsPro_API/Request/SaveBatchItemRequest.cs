using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_API.Request
{
    public class SaveBatchItemRequest
    {
        public List<int> Ids { get; set; }
        public int VendorId { get; set; }
        public string BatchDate { get; set; }
    }
}

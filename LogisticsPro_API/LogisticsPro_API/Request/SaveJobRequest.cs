using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_API.Request
{
    public class SaveJobRequest
    {
        public int Id { get; set; }
        public string JobDescription { get; set; }
        public int CustomerId { get; set; }
        public string CustomerRef { get; set; }
        public string EffectiveDate { get; set; }
        public string Remarks { get; set; }
    }
}

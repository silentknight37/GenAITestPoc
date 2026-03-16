using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class BatchLineItemResponse
    {
        public int Id { get; set; }
        public string BatchNo { get; set; }
        public string SuplierName { get; set; }
        public DateTime? BatchDate { get; set; }
        public string BatchDateStn
        {
            get
            {
                return BatchDate == null ? "" : BatchDate.Value.ToString("MM/dd/yyyy");
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class Batch
    {
        public Batch()
        {
            BatchItems = new List<BatchItem>();
            BatchTransportItems=new List<BatchLineItem>();
        }
        public int Id { get; set; }
        public string BatchCode { get; set; }
        public int? VendorId { get; set; }
        public string VendorName { get; set; }
        public DateTime? BatchDate { get; set; }
        public List<BatchItem> BatchItems { get; set; }
        public List<BatchLineItem> BatchTransportItems { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class Dashboard_Chart_1
    {
        public Dashboard_Chart_1()
        {
            Generated = new List<int?>();
            Paid = new List<int?>();
            UnPaid = new List<int?>();
        }

        public List<int?> Generated { get; set; }
        public List<int?> Paid { get; set; }
        public List<int?> UnPaid { get; set; }
    }
}

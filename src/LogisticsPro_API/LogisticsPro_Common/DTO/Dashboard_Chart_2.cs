using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class Dashboard_Chart_2
    {
        public Dashboard_Chart_2    ()
        {
            Monthly = new List<int?>();
            Yearly = new List<int?>();
        }

        public List<int?> Monthly { get; set; }
        public List<int?> Yearly { get; set; }
    }
}

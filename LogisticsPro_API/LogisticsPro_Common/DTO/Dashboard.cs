using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class Dashboard
    {
        public Dashboard()
        {
            CustomersMonthly = new List<Customer>();
            CustomersYearly = new List<Customer>();
            VendorsMonthly = new List<Vendor>();
            VendorsYearly = new List<Vendor>();
            Events = new List<Event>();
            UpcomingTransfers = new List<JobCardTransportation>();
            UpcomingHotels=new List<JobCardHotel>();
            UpcomingVisas=new List<JobCardVisa>();
            UpcomingMiscellaneours = new List<JobCardMiscellaneous>();
            UnBatchTransfers = new List<JobCardTransportation>();
            Chart_1 = new Dashboard_Chart_1();
            Chart_2 = new Dashboard_Chart_2();
        }

        public List<Customer> CustomersMonthly { get; set; }
        public List<Customer> CustomersYearly { get; set; }
        public List<Vendor> VendorsMonthly { get; set; }
        public List<Vendor> VendorsYearly { get; set; }
        public List<Event> Events { get; set; }
        public Dashboard_Chart_1 Chart_1 { get; set; }
        public Dashboard_Chart_2 Chart_2 { get; set; }
        public List<JobCardTransportation> UpcomingTransfers { get; set; }
        public List<JobCardHotel> UpcomingHotels { get; set; }
        public List<JobCardVisa> UpcomingVisas { get; set; }
        public List<JobCardMiscellaneous> UpcomingMiscellaneours { get; set; }
        public List<JobCardTransportation> UnBatchTransfers { get; set; }
        public int TotalInvoice { get; set; }
        public decimal TotalInvoiceValue { get; set; }
        public int TotalGeneratedInvoice { get; set; }
        public int TotalPaidInvoice { get; set; }

        public int TotalUnPaidInvoice { get; set; }
        public int TotalJobCardMonthly { get; set; }
        public decimal TotalJobCardValueMonthly { get; set; }
        public decimal TotalJobCardValueYearly { get; set; }
        public int TotalJobCardYearly { get;set; }
        public double PaidPresentage
        {
            get
            {
                if(TotalPaidInvoice==0 || TotalInvoice == 0)
                {
                    return 0;
                }
                double v = ((double)TotalPaidInvoice / (double)TotalInvoice) * 100;
                return v;
            }
        }
    }
}

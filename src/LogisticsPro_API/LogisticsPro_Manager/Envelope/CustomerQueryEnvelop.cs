using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Envelope
{
    public class CustomerQueryEnvelop
    {
        public CustomerQueryEnvelop(List<Customer> customers)
        {
            this.Customers = customers;
        }

        public List<Customer> Customers { get; set; }
    }
}

using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class CustomerEventQueryHandler:IRequestHandler<CustomerEventQuery, CustomerQueryEnvelop>
    {
        private readonly IMasterFactory masterFactory;

        public CustomerEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.masterFactory = new MasterFactory(dB_LogisticsproContext);
        }

        public async Task<CustomerQueryEnvelop> Handle(CustomerEventQuery customerEventQuery, CancellationToken cancellationToken)
        {
            var masterDomain = this.masterFactory.Create();
            var customers = await masterDomain.GetCustomers();

            return new CustomerQueryEnvelop(customers);
        }
    }
}

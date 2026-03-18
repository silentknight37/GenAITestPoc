using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class VendorEventQueryHandler : IRequestHandler<VendorEventQuery, VendorQueryEnvelop>
    {
        private readonly IMasterFactory masterFactory;

        public VendorEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.masterFactory = new MasterFactory(dB_LogisticsproContext);
        }

        public async Task<VendorQueryEnvelop> Handle(VendorEventQuery vendorEventQuery, CancellationToken cancellationToken)
        {
            var masterDomain = this.masterFactory.Create();
            var vendors = await masterDomain.GetVendors();

            return new VendorQueryEnvelop(vendors);
        }
    }
}

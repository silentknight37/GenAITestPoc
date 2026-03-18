using LogisticsPro_Common.DTO;
using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class PnLEventQueryHandler : IRequestHandler<PnLEventQuery, PnLEnvelop>
    {
        private readonly ILogisticsFactory logisticsFactory;

        public PnLEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.logisticsFactory = new LogisticsFactory(dB_LogisticsproContext);
        }

        public async Task<PnLEnvelop> Handle(PnLEventQuery pnLEventQuery, CancellationToken cancellationToken)
        {
            var logisticsDomain = this.logisticsFactory.Create();
            var pnl = await logisticsDomain.GetPnL(pnLEventQuery.CustomerId, pnLEventQuery.DateFrom, pnLEventQuery.DateTo,pnLEventQuery.JobCardNumber);
           
            return new PnLEnvelop(pnl);
        }
    }
}

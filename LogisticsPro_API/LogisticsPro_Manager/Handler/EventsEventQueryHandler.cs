using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class EventsEventQueryHandler : IRequestHandler<EventEventQuery, EventQueryEnvelop>
    {
        private readonly IMasterFactory masterFactory;

        public EventsEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.masterFactory = new MasterFactory(dB_LogisticsproContext);
        }

        public async Task<EventQueryEnvelop> Handle(EventEventQuery eventEventQuery, CancellationToken cancellationToken)
        {
            var masterDomain = this.masterFactory.Create();
            var events = await masterDomain.GetEvents();

            return new EventQueryEnvelop(events);
        }
    }
}

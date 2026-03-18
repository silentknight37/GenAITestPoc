using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class UserEventQueryHandler : IRequestHandler<UserEventQuery, UserQueryEnvelop>
    {
        private readonly IMasterFactory masterFactory;

        public UserEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.masterFactory = new MasterFactory(dB_LogisticsproContext);
        }

        public async Task<UserQueryEnvelop> Handle(UserEventQuery vendorEventQuery, CancellationToken cancellationToken)
        {
            var masterDomain = this.masterFactory.Create();
            var users = await masterDomain.GetUsers();

            return new UserQueryEnvelop(users);
        }
    }
}

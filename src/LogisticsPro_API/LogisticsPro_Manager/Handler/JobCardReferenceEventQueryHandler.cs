using LogisticsPro_Common.DTO;
using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class JobCardReferenceEventQueryHandler : IRequestHandler<JobCardReferenceEventQuery, JobCardReferanceQueryEnvelop>
    {
        private readonly IMasterFactory masterFactory;

        public JobCardReferenceEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.masterFactory = new MasterFactory(dB_LogisticsproContext);
        }

        public async Task<JobCardReferanceQueryEnvelop> Handle(JobCardReferenceEventQuery referenceEventQuery, CancellationToken cancellationToken)
        {
            var masterDomain = this.masterFactory.Create();
            var jobCards = await masterDomain.GetJobCards(referenceEventQuery.CustomerId);

            var referenceRefDatas = jobCards.Select(i => new ReferenceData { value = i.Id, code = $"{i.JobCardCode} - {i.JobCardDescription}" }).ToList();

            return new JobCardReferanceQueryEnvelop(referenceRefDatas);
        }
    }
}

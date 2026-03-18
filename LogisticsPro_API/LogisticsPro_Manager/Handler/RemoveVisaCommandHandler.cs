using LogisticsPro_Common.Common;
using LogisticsPro_Common.DTO;
using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Command;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using MediatR;

namespace LogisticsPro_Manager.Handler
{
    public class RemoveVisaCommandHandler : IRequestHandler<RemoveVisalCommand, RequestRemoveEnvelop>
    {
        private readonly IJobFactory jobFactory;

        public RemoveVisaCommandHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.jobFactory = new JobFactory(dB_LogisticsproContext);
        }

        public async Task<RequestRemoveEnvelop> Handle(RemoveVisalCommand removeVisalCommand,CancellationToken cancellationToken)
        {
            var jobDomain = this.jobFactory.Create();
            try
            {
                var response = await jobDomain.RemoveVisa(removeVisalCommand.Id);

                if (!response)
                {
                    var errorMessage = "Request fail due to invalid user";
                    Error error = new Error(ErrorType.UNAUTHORIZED, errorMessage);
                    return new RequestRemoveEnvelop(false, string.Empty, error);
                }

                return new RequestRemoveEnvelop(response, "Request process successfully", null);

            }
            catch (Exception e)
            {
                var errorMessage = e.Message;
                Error error = new Error(ErrorType.BAD_REQUEST, errorMessage);
                return new RequestRemoveEnvelop(false, string.Empty, error);
            }
        }
    }
}

using LogisticsPro_Common.Common;
using LogisticsPro_Common.DTO;
using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Command;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using MediatR;

namespace LogisticsPro_Manager.Handler
{
    public class CloseJobCardCommandHandler : IRequestHandler<CloseJobCardCommand, RequestSaveEnvelop>
    {
        private readonly IJobFactory jobFactory;

        public CloseJobCardCommandHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.jobFactory = new JobFactory(dB_LogisticsproContext);
        }

        public async Task<RequestSaveEnvelop> Handle(CloseJobCardCommand closeJobCardCommand,CancellationToken cancellationToken)
        {
            var jobDomain = this.jobFactory.Create();
            try
            {
                var closeJobCardRequest = new CloseJobCardRequest
                {  Id= closeJobCardCommand.Id,
                   UserId= closeJobCardCommand.UserId
                };

                var response = await jobDomain.CloseJobCard(closeJobCardRequest);

                if (!(response>0))
                {
                    var errorMessage = "Request fail due to invalid user";
                    Error error = new Error(ErrorType.UNAUTHORIZED, errorMessage);
                    return new RequestSaveEnvelop(false, string.Empty, error);
                }

                return new RequestSaveEnvelop(response>0, "Request process successfully", response, null);

            }
            catch (Exception e)
            {
                var errorMessage = e.Message;
                Error error = new Error(ErrorType.BAD_REQUEST, errorMessage);
                return new RequestSaveEnvelop(false, string.Empty, error);
            }
        }
    }
}

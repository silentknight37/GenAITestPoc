using LogisticsPro_Common.Common;
using LogisticsPro_Common.DTO;
using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Command;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using MediatR;

namespace LogisticsPro_Manager.Handler
{
    public class SaveJobCommandHandler : IRequestHandler<SaveJobCommand, RequestSaveEnvelop>
    {
        private readonly IJobFactory jobFactory;

        public SaveJobCommandHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.jobFactory = new JobFactory(dB_LogisticsproContext);
        }

        public async Task<RequestSaveEnvelop> Handle(SaveJobCommand saveJobCommand,CancellationToken cancellationToken)
        {
            var jobDomain = this.jobFactory.Create();
            try
            {
                var jobSaveRequest = new JobSaveRequest
                {  Id=saveJobCommand.Id,
                   CustomerId= saveJobCommand.CustomerId,
                   CustomerRef=saveJobCommand.CustomerRef,
                   JobDescription=saveJobCommand.JobDescription,
                   EffectiveDate=saveJobCommand.EffectiveDate,
                   Remarks=saveJobCommand.Remarks,
                   UserId=saveJobCommand.UserId
                 };

                var response = await jobDomain.SaveJob(jobSaveRequest);

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

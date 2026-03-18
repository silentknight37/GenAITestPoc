using LogisticsPro_Common.Common;
using LogisticsPro_Common.DTO;
using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Command;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using MediatR;

namespace LogisticsPro_Manager.Handler
{
    public class SaveJobCardMiscellaneousCommandHandler : IRequestHandler<SaveJobMiscellaneousCommand, RequestSaveEnvelop>
    {
        private readonly IJobFactory jobFactory;

        public SaveJobCardMiscellaneousCommandHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.jobFactory = new JobFactory(dB_LogisticsproContext);
        }

        public async Task<RequestSaveEnvelop> Handle(SaveJobMiscellaneousCommand saveJobMiscellaneousCommand,CancellationToken cancellationToken)
        {
            var jobDomain = this.jobFactory.Create();
            try
            {
                var jobCardMiscellaneousRequest = new JobCardMiscellaneousRequest
                {
                    Id = saveJobMiscellaneousCommand.Id,
                    JobCardId = saveJobMiscellaneousCommand.JobCardId,
                    VendorId=saveJobMiscellaneousCommand.VendorId,
                    PaxName = saveJobMiscellaneousCommand.PaxName,
                    PaxNumber = saveJobMiscellaneousCommand.PaxNumber,
                    Description = saveJobMiscellaneousCommand.Description,
                    Remarks = saveJobMiscellaneousCommand.Remarks,
                    MisDate = saveJobMiscellaneousCommand.MisDate,
                    IsVatIncludedCost= saveJobMiscellaneousCommand.IsVatIncludedCost,
                    IsVatIncludedSell= saveJobMiscellaneousCommand.IsVatIncludedSell,
                    CostBaseAmount = saveJobMiscellaneousCommand.CostBaseAmount,
                    CostTaxAmount = saveJobMiscellaneousCommand.CostTaxAmount,
                    SellBaseAmount = saveJobMiscellaneousCommand.SellBaseAmount,
                    SellTaxAmount = saveJobMiscellaneousCommand.SellTaxAmount,
                    IsFinance=saveJobMiscellaneousCommand.IsFinance,
                    UserId =saveJobMiscellaneousCommand.UserId
                };

                var response = await jobDomain.SaveJobMiscellaneous(jobCardMiscellaneousRequest);

                if (!(response))
                {
                    var errorMessage = "Request fail due to invalid user";
                    Error error = new Error(ErrorType.UNAUTHORIZED, errorMessage);
                    return new RequestSaveEnvelop(false, string.Empty, error);
                }

                return new RequestSaveEnvelop(response, "Request process successfully", null);

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

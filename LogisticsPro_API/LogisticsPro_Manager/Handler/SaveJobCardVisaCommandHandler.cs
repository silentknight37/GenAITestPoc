using LogisticsPro_Common.Common;
using LogisticsPro_Common.DTO;
using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Command;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using MediatR;

namespace LogisticsPro_Manager.Handler
{
    public class SaveJobCardVisaCommandHandler : IRequestHandler<SaveJobCardVisaCommand, RequestSaveEnvelop>
    {
        private readonly IJobFactory jobFactory;

        public SaveJobCardVisaCommandHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.jobFactory = new JobFactory(dB_LogisticsproContext);
        }

        public async Task<RequestSaveEnvelop> Handle(SaveJobCardVisaCommand saveJobCardVisaCommand,CancellationToken cancellationToken)
        {
            var jobDomain = this.jobFactory.Create();
            try
            {
                var jobCardVisaRequest = new JobCardVisaRequest
                {
                    Id = saveJobCardVisaCommand.Id,
                    JobCardId = saveJobCardVisaCommand.JobCardId,
                    VendorId = saveJobCardVisaCommand.VendorId,
                    PaxName = saveJobCardVisaCommand.PaxName,
                    PassportNo = saveJobCardVisaCommand.PassportNo,
                    VisaTypeId = saveJobCardVisaCommand.VisaTypeId,
                    IsVatIncludedCost=saveJobCardVisaCommand.IsVatIncludedCost,
                    IsVatIncludedSell= saveJobCardVisaCommand.IsVatIncludedSell,
                    CostBaseAmount = saveJobCardVisaCommand.CostBaseAmount,
                    CostTaxAmount = saveJobCardVisaCommand.CostTaxAmount,
                    SellBaseAmount = saveJobCardVisaCommand.SellBaseAmount,
                    SellTaxAmount = saveJobCardVisaCommand.SellTaxAmount,
                    Remarks = saveJobCardVisaCommand.Remarks,
                    Nationality = saveJobCardVisaCommand.Nationality,
                    UserId=saveJobCardVisaCommand.UserId
                };

                var response = await jobDomain.SaveJobVisa(jobCardVisaRequest);

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

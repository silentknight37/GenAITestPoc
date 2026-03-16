using LogisticsPro_Common.Common;
using LogisticsPro_Common.DTO;
using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Command;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using MediatR;

namespace LogisticsPro_Manager.Handler
{
    public class SaveBatchItemCommandHandler : IRequestHandler<SaveBatchItemCommand, BatchRequestSaveEnvelop>
    {
        private readonly ILogisticsFactory logisticsFactory;

        public SaveBatchItemCommandHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.logisticsFactory = new LogisticsFactory(dB_LogisticsproContext);
        }

        public async Task<BatchRequestSaveEnvelop> Handle(SaveBatchItemCommand saveBatchItemCommand,CancellationToken cancellationToken)
        {
            var logisticsDomain = this.logisticsFactory.Create();
            try
            {
                var batchItemSaveRequest = new BatchItemSaveRequest
                {
                    Ids = saveBatchItemCommand.Ids,
                    BatchDate = saveBatchItemCommand.BatchDate,
                    VendorId = saveBatchItemCommand.VendorId,
                    UserId=saveBatchItemCommand.UserId
                };

                var response = await logisticsDomain.SaveBatchItem(batchItemSaveRequest);

                if (!(response.Id>0))
                {
                    var errorMessage = "Request fail due to invalid user";
                    Error error = new Error(ErrorType.UNAUTHORIZED, errorMessage);
                    return new BatchRequestSaveEnvelop(false, string.Empty, error);
                }

                return new BatchRequestSaveEnvelop(response.Id >0, "Request process successfully", response, null);

            }
            catch (Exception e)
            {
                var errorMessage = e.Message;
                Error error = new Error(ErrorType.BAD_REQUEST, errorMessage);
                return new BatchRequestSaveEnvelop(false, string.Empty, error);
            }
        }
    }
}

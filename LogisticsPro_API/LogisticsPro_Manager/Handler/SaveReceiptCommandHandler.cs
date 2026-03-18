using LogisticsPro_Common.Common;
using LogisticsPro_Common.DTO;
using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Command;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using MediatR;

namespace LogisticsPro_Manager.Handler
{
    public class SaveReceiptCommandHandler : IRequestHandler<SaveReceiptCommand, RequestSaveEnvelop>
    {
        private readonly ILogisticsFactory logisticsFactory;

        public SaveReceiptCommandHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.logisticsFactory = new LogisticsFactory(dB_LogisticsproContext);
        }

        public async Task<RequestSaveEnvelop> Handle(SaveReceiptCommand saveReceiptCommand,CancellationToken cancellationToken)
        {
            var logisticsDomain = this.logisticsFactory.Create();
            try
            {
                var receiptSaveRequest = new ReceiptSaveRequest
                {
                   Id= saveReceiptCommand.Id,
                   ReceiptDate= saveReceiptCommand.ReceiptDate,
                   Remark= saveReceiptCommand.Remark,
                   Amount = saveReceiptCommand.Amount,
                   PaymentMethod = saveReceiptCommand.PaymentMethod,
                   InvoiceId = saveReceiptCommand.InvoiceId,
                   UserId = saveReceiptCommand.UserId
                };
                saveReceiptCommand.UpdateRecords.ForEach(i => receiptSaveRequest.UpdateRecords.Add(new LogisticsPro_Common.DTO.UpdateRecords
                {
                    Id = i.Id,
                    ServiceType= i.ServiceType,
                    AllocatedAmount = i.AllocatedAmount
                }));
                
                var response = await logisticsDomain.SaveReceipt(receiptSaveRequest);

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

using LogisticsPro_Common.Common;
using LogisticsPro_Common.DTO;
using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Command;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using MediatR;

namespace LogisticsPro_Manager.Handler
{
    public class SaveProformaInvoiceReceiptCommandHandler : IRequestHandler<SaveProformaInvoiceReceiptCommand, RequestSaveEnvelop>
    {
        private readonly ILogisticsFactory logisticsFactory;

        public SaveProformaInvoiceReceiptCommandHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.logisticsFactory = new LogisticsFactory(dB_LogisticsproContext);
        }

        public async Task<RequestSaveEnvelop> Handle(SaveProformaInvoiceReceiptCommand saveProformaInvoiceReceiptCommand,CancellationToken cancellationToken)
        {
            var logisticsDomain = this.logisticsFactory.Create();
            try
            {
                var proformaInvoiceReceiptSaveRequest = new ProformaInvoiceReceiptSaveRequest
                {
                   Id= saveProformaInvoiceReceiptCommand.Id,
                   ReceiptDate= saveProformaInvoiceReceiptCommand.ReceiptDate,
                   Remark= saveProformaInvoiceReceiptCommand.Remark,
                   Amount = saveProformaInvoiceReceiptCommand.Amount,
                   JobCardId = saveProformaInvoiceReceiptCommand.JobCardId,
                   PaymentMethod = saveProformaInvoiceReceiptCommand.PaymentMethod,
                   ProformaInvoiceId = saveProformaInvoiceReceiptCommand.ProformaInvoiceId,
                   UserId = saveProformaInvoiceReceiptCommand.UserId
                };

                var response = await logisticsDomain.SaveProformaInvoiceReceipt(proformaInvoiceReceiptSaveRequest);

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

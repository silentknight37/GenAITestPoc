using LogisticsPro_Common.Common;
using LogisticsPro_Common.DTO;
using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Command;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using MediatR;

namespace LogisticsPro_Manager.Handler
{
    public class SaveProformaInvoiceCommandHandler : IRequestHandler<SaveProformaInvoiceCommand, RequestSaveEnvelop>
    {
        private readonly ILogisticsFactory logisticsFactory;

        public SaveProformaInvoiceCommandHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.logisticsFactory = new LogisticsFactory(dB_LogisticsproContext);
        }

        public async Task<RequestSaveEnvelop> Handle(SaveProformaInvoiceCommand saveProformaInvoiceCommand,CancellationToken cancellationToken)
        {
            var logisticsDomain = this.logisticsFactory.Create();
            try
            {
                var proformaInvoiceSaveRequest = new ProformaInvoiceSaveRequest
                {
                    InvoiceDate = saveProformaInvoiceCommand.InvoiceDate,
                    CustomerId = saveProformaInvoiceCommand.CustomerId,
                    InvoiceDueDate= saveProformaInvoiceCommand.InvoiceDueDate,
                    InvoiceAmount= saveProformaInvoiceCommand.InvoiceAmount,
                    JobCardId = saveProformaInvoiceCommand.JobCardId,
                    Description= saveProformaInvoiceCommand.Description,
                    UserId= saveProformaInvoiceCommand.UserId
                };

                var response = await logisticsDomain.SaveProformaInvoice(proformaInvoiceSaveRequest);

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

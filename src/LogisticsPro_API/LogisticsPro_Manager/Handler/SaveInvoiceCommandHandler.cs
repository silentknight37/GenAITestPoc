using LogisticsPro_Common.Common;
using LogisticsPro_Common.DTO;
using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Command;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using MediatR;

namespace LogisticsPro_Manager.Handler
{
    public class SaveInvoiceCommandHandler : IRequestHandler<SaveInvoiceCommand, RequestSaveEnvelop>
    {
        private readonly ILogisticsFactory logisticsFactory;

        public SaveInvoiceCommandHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.logisticsFactory = new LogisticsFactory(dB_LogisticsproContext);
        }

        public async Task<RequestSaveEnvelop> Handle(SaveInvoiceCommand saveInvoiceCommand,CancellationToken cancellationToken)
        {
            var logisticsDomain = this.logisticsFactory.Create();
            try
            {
                var invoiceSaveRequest = new InvoiceSaveRequest
                {
                    TransportationIds = saveInvoiceCommand.TransportationIds,
                    HotelIds = saveInvoiceCommand.HotelIds,
                    VisaIds = saveInvoiceCommand.VisaIds,
                    MiscellaneousIds = saveInvoiceCommand.MiscellaneousIds,
                    InvoiceDate = saveInvoiceCommand.InvoiceDate,
                    CustomerId = saveInvoiceCommand.CustomerId,
                    InvoiceDueDate= saveInvoiceCommand.InvoiceDueDate,
                    InvoiceAmount= saveInvoiceCommand.InvoiceAmount,
                    Remarks = saveInvoiceCommand.Remarks,
                    TransportDescription =saveInvoiceCommand.TransportDescription,
                    HotelDescription = saveInvoiceCommand.HotelDescription,
                    VisaDescription = saveInvoiceCommand.VisaDescription,
                    MiscellaneousDescription = saveInvoiceCommand.MiscellaneousDescription,
                    PerformaInvoiceIds =saveInvoiceCommand.PerformaInvoiceIds,
                    UserId=saveInvoiceCommand.UserId
                };

                var response = await logisticsDomain.SaveInvoice(invoiceSaveRequest);

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

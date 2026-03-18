using LogisticsPro_Common.Common;
using LogisticsPro_Common.DTO;
using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Command;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using MediatR;

namespace LogisticsPro_Manager.Handler
{
    public class VoidInvoiceReceiptCommandHandler : IRequestHandler<VoidInvoiceReceiptCommand, RequestVoidEnvelop>
    {
        private readonly ILogisticsFactory logisticsFactory;

        public VoidInvoiceReceiptCommandHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.logisticsFactory = new LogisticsFactory(dB_LogisticsproContext);
        }

        public async Task<RequestVoidEnvelop> Handle(VoidInvoiceReceiptCommand voidInvoiceReceiptCommand,CancellationToken cancellationToken)
        {
            var logisticsDomain = this.logisticsFactory.Create();
            try
            {
                var response = await logisticsDomain.VoidInvoiceReceipt(voidInvoiceReceiptCommand.Id);

                if (!response)
                {
                    var errorMessage = "Request fail due to invalid user";
                    Error error = new Error(ErrorType.UNAUTHORIZED, errorMessage);
                    return new RequestVoidEnvelop(false, string.Empty, error);
                }

                return new RequestVoidEnvelop(response, "Request process successfully", null);

            }
            catch (Exception e)
            {
                var errorMessage = e.Message;
                Error error = new Error(ErrorType.BAD_REQUEST, errorMessage);
                return new RequestVoidEnvelop(false, string.Empty, error);
            }
        }
    }
}

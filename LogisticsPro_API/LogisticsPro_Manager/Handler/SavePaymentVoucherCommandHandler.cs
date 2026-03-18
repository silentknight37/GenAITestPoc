using LogisticsPro_Common.Common;
using LogisticsPro_Common.DTO;
using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Command;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using MediatR;

namespace LogisticsPro_Manager.Handler
{
    public class SavePaymentVoucherCommandHandler : IRequestHandler<SavePaymentVoucherCommand, RequestSaveEnvelop>
    {
        private readonly ILogisticsFactory logisticsFactory;

        public SavePaymentVoucherCommandHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.logisticsFactory = new LogisticsFactory(dB_LogisticsproContext);
        }

        public async Task<RequestSaveEnvelop> Handle(SavePaymentVoucherCommand savePaymentVoucherCommand,CancellationToken cancellationToken)
        {
            var logisticsDomain = this.logisticsFactory.Create();
            try
            {
                var paymentVoucherSaveRequest = new PaymentVoucherSaveRequest
                {
                    TransportationIds = savePaymentVoucherCommand.TransportationIds,
                    HotelIds = savePaymentVoucherCommand.HotelIds,
                    VisaIds = savePaymentVoucherCommand.VisaIds,
                    MiscellaneousIds = savePaymentVoucherCommand.MiscellaneousIds,
                    PaymentVoucherDate = savePaymentVoucherCommand.PaymentVoucherDate,
                    VendorId = savePaymentVoucherCommand.VendorId,
                    Invoice=savePaymentVoucherCommand.Invoice,
                    PaymentVoucherAmount= savePaymentVoucherCommand.PaymentVoucherAmount,
                    Remarks = savePaymentVoucherCommand.Remarks,
                    UserId=savePaymentVoucherCommand.UserId
                };

                var response = await logisticsDomain.SavePaymentVoucher(paymentVoucherSaveRequest);

                if (response<=0)
                {
                    var errorMessage = "Request fail due to invalid user";
                    Error error = new Error(ErrorType.UNAUTHORIZED, errorMessage);
                    return new RequestSaveEnvelop(false, string.Empty, error);
                }

                return new RequestSaveEnvelop(response <= 0, "Request process successfully", response, null);

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

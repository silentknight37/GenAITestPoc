using LogisticsPro_Common.Common;
using LogisticsPro_Common.DTO;
using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Command;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using MediatR;

namespace LogisticsPro_Manager.Handler
{
    public class SaveJobCardHotelCommandHandler : IRequestHandler<SaveJobCardHotelCommand, RequestSaveEnvelop>
    {
        private readonly IJobFactory jobFactory;

        public SaveJobCardHotelCommandHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.jobFactory = new JobFactory(dB_LogisticsproContext);
        }

        public async Task<RequestSaveEnvelop> Handle(SaveJobCardHotelCommand saveJobCardHotelCommand,CancellationToken cancellationToken)
        {
            var jobDomain = this.jobFactory.Create();
            try
            {
                var jobCardHotelRequest = new JobCardHotelRequest
                {
                    Id = saveJobCardHotelCommand.Id,
                    JobCardId = saveJobCardHotelCommand.JobCardId,
                    Adults = saveJobCardHotelCommand.Adults,
                    Children = saveJobCardHotelCommand.Children,
                    Infants = saveJobCardHotelCommand.Infants,
                    PaxName = saveJobCardHotelCommand.PaxName,
                    HotelName = saveJobCardHotelCommand.HotelName,
                    HotelAddress1 = saveJobCardHotelCommand.HotelAddress1,
                    HotelAddress2= saveJobCardHotelCommand.HotelAddress2,
                    VendorId = saveJobCardHotelCommand.VendorId,
                    CheckIn = saveJobCardHotelCommand.CheckIn,
                    CheckOut = saveJobCardHotelCommand.CheckOut,
                    IsVatIncludedCost= saveJobCardHotelCommand.IsVatIncludedCost,
                    IsVatIncludedSell= saveJobCardHotelCommand.IsVatIncludedSell,
                    CostBaseAmount = saveJobCardHotelCommand.CostBaseAmount,
                    CostTaxAmount = saveJobCardHotelCommand.CostTaxAmount,
                    SellBaseAmount = saveJobCardHotelCommand.SellBaseAmount,
                    SellTaxAmount = saveJobCardHotelCommand.SellTaxAmount,
                    Remarks = saveJobCardHotelCommand.Remarks,
                    HotelConfirmation= saveJobCardHotelCommand.HotelConfirmation,
                    RoomType= saveJobCardHotelCommand.RoomType,
                    UserId= saveJobCardHotelCommand.UserId
                };

                var response = await jobDomain.SaveJobHotel(jobCardHotelRequest);

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

using LogisticsPro_Common.Common;
using LogisticsPro_Common.DTO;
using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Command;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using MediatR;

namespace LogisticsPro_Manager.Handler
{
    public class SaveJobCardTransportationCommandHandler : IRequestHandler<SaveJobCardTransportationCommand, RequestSaveEnvelop>
    {
        private readonly IJobFactory jobFactory;

        public SaveJobCardTransportationCommandHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.jobFactory = new JobFactory(dB_LogisticsproContext);
        }

        public async Task<RequestSaveEnvelop> Handle(SaveJobCardTransportationCommand saveJobCardTransportationCommand,CancellationToken cancellationToken)
        {
            var jobDomain = this.jobFactory.Create();
            try
            {
                var jobCardTransportationRequest = new JobCardTransportationRequest
                {
                    Id = saveJobCardTransportationCommand.Id,
                    JobCardId = saveJobCardTransportationCommand.JobCardId,
                    CustomerRef = saveJobCardTransportationCommand.CustomerRef,
                    Adults = saveJobCardTransportationCommand.Adults,
                    Children = saveJobCardTransportationCommand.Children,
                    Infants = saveJobCardTransportationCommand.Infants,
                    PaxName = saveJobCardTransportationCommand.PaxName,
                    PickupLocation = saveJobCardTransportationCommand.PickupLocation,
                    PickupTime = saveJobCardTransportationCommand.PickupTime,
                    DropoffLocation = saveJobCardTransportationCommand.DropoffLocation,
                    FlightNo = saveJobCardTransportationCommand.FlightNo,
                    FlightTime = saveJobCardTransportationCommand.FlightTime,
                    VehicleType = saveJobCardTransportationCommand.VehicleType,
                    CostBaseAmount = saveJobCardTransportationCommand.CostBaseAmount,
                    CostTaxAmount = saveJobCardTransportationCommand.CostTaxAmount,
                    SellBaseAmount = saveJobCardTransportationCommand.SellBaseAmount,
                    SellTaxAmount = saveJobCardTransportationCommand.SellTaxAmount,
                    IsVatIncludedCost = saveJobCardTransportationCommand.IsVatIncludedCost,
                    IsVatIncludedSell = saveJobCardTransportationCommand.IsVatIncludedSell,
                    Extras= saveJobCardTransportationCommand.Extras,
                    ExtrasTaxAmount= saveJobCardTransportationCommand.ExtrasTaxAmount,
                    Water = saveJobCardTransportationCommand.Water,
                    WaterTaxAmount= saveJobCardTransportationCommand.WaterTaxAmount,
                    Parking= saveJobCardTransportationCommand.Parking,
                    ParkingTaxAmount= saveJobCardTransportationCommand.ParkingTaxAmount,
                    ExtrasSell = saveJobCardTransportationCommand.ExtrasSell,
                    ExtrasTaxAmountSell = saveJobCardTransportationCommand.ExtrasTaxAmountSell,
                    WaterSell = saveJobCardTransportationCommand.WaterSell,
                    WaterTaxAmountSell= saveJobCardTransportationCommand.WaterTaxAmountSell,
                    ParkingSell = saveJobCardTransportationCommand.ParkingSell,
                    ParkingTaxAmountSell= saveJobCardTransportationCommand.ParkingTaxAmountSell,
                    Remarks = saveJobCardTransportationCommand.Remarks,
                    UserId= saveJobCardTransportationCommand.UserId
                };

                var response = await jobDomain.SaveJobTransportation(jobCardTransportationRequest);

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

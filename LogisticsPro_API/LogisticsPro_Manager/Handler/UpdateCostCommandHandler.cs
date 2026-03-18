using LogisticsPro_Common.Common;
using LogisticsPro_Common.DTO;
using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Command;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using MediatR;

namespace LogisticsPro_Manager.Handler
{
    public class UpdateCostCommandHandler : IRequestHandler<UpdateCostCommand, RequestSaveEnvelop>
    {
        private readonly IJobFactory jobFactory;

        public UpdateCostCommandHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.jobFactory = new JobFactory(dB_LogisticsproContext);
        }

        public async Task<RequestSaveEnvelop> Handle(UpdateCostCommand updateCostCommand,CancellationToken cancellationToken)
        {
            var jobDomain = this.jobFactory.Create();
            try
            {
                var updateCostRequest = new UpdateCostRequest
                {
                    Id = updateCostCommand.Id,
                    CostBaseAmount = updateCostCommand.CostBaseAmount,
                    CostTaxAmount = updateCostCommand.CostTaxAmount,
                    IsVatIncludedCost = updateCostCommand.IsVatIncludedCost,
                    IsVatIncludedSell = updateCostCommand.IsVatIncludedSell,
                    SellBaseAmount = updateCostCommand.SellBaseAmount,
                    SellTaxAmount = updateCostCommand.SellTaxAmount,
                    ExtrasTaxAmount = updateCostCommand.ExtrasTaxAmount,
                    Extras = updateCostCommand.Extras,
                    Parking = updateCostCommand.Parking,
                    Water = updateCostCommand.Water,
                    ParkingSell = updateCostCommand.ParkingSell,
                    ExtrasSell = updateCostCommand.ExtrasSell,
                    WaterSell = updateCostCommand.WaterSell,
                    ExtrasTaxAmountSell = updateCostCommand.ExtrasTaxAmountSell,
                    UserId = updateCostCommand.UserId
                };

                var response = await jobDomain.UpdateCost(updateCostRequest);

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

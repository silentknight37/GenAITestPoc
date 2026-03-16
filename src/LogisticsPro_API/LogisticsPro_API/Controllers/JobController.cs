using LogisticsPro_API.Request;
using LogisticsPro_Common.DTO;
using LogisticsPro_Manager.Command;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Request = LogisticsPro_API.Request;

namespace LogisticsPro_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JobController : BaseController
    {
        private readonly IMediator mediator;
        public JobController(IMediator mediator, JwtService jwtservice) : base(jwtservice)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetJobs")]
        public async Task<JsonResult> GetJobs(bool isFirstLoad,string? jobCardCode,string? jobCardDescription, string? customerId, DateTime? effectiveDateFrom,DateTime? effectiveDateTo, string? statusId)
        {
            var userId = GetUserIdFromToken();
            var customerIds = customerId == null ? new List<int>() : customerId.Split(',').Select(Int32.Parse).ToList();
            var statusIds = statusId == null ? new List<int>() : statusId.Split(',').Select(Int32.Parse).ToList();
            var jobs = await mediator.Send(new JobEventQuery(isFirstLoad,(int)userId, jobCardCode,jobCardDescription, customerIds, effectiveDateFrom,effectiveDateTo, statusIds));
            return new JsonResult(jobs);
        }

        [HttpGet]
        [Route("GetJobById")]
        public async Task<JsonResult> GetJobById(int id)
        {
            var userId = GetUserIdFromToken();
            var job = await mediator.Send(new JobCardEventQuery((int)userId, id));
            return new JsonResult(job);
        }

        [HttpPost]
        [Route("SaveJob")]
        public async Task<JsonResult> SaveJob(SaveJobRequest saveJobRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new SaveJobCommand(saveJobRequest.Id,saveJobRequest.JobDescription, saveJobRequest.CustomerId, saveJobRequest.CustomerRef, DateTime.Parse(saveJobRequest.EffectiveDate), saveJobRequest.Remarks, (int)userId));

            return new JsonResult(result);
        }

        [HttpPost]
        [Route("RemoveJobCard")]
        public async Task<JsonResult> RemoveJobCard(RemoveRequest removeRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new RemoveJobCardCommand(removeRequest.Id, (int)userId));

            return new JsonResult(result);
        }

        [HttpGet]
        [Route("GetJobTransportationsByJobCardId")]
        public async Task<JsonResult> GetJobTransportationsByJobCardId(int id)
        {
            var userId = GetUserIdFromToken();
            var jobCardTransportation = await mediator.Send(new JobCardTransportationsEventQuery((int)userId, id));
            return new JsonResult(jobCardTransportation);
        }

        [HttpGet]
        [Route("GetJobTransportations")]
        public async Task<JsonResult> GetJobTransportations(string? jobCardCode, string? bookingRef, string? batchNo, int? customerId, int? vendorId, string? clientRef, DateTime? dateFrom, DateTime? dateTo)
        {
            var userId = GetUserIdFromToken();
            var jobCardTransportation = await mediator.Send(new CostTransportationsEventQuery((int)userId, jobCardCode, bookingRef,batchNo,customerId,vendorId,clientRef,dateFrom,dateTo));
            return new JsonResult(jobCardTransportation);
        }

        [HttpGet]
        [Route("GetJobLineItems")]
        public async Task<JsonResult> GetJobLineItems(string? jobCardCode, string? bookingRef, string? batchNo, int? customerId, int? vendorId, string? clientRef, DateTime? dateFrom, DateTime? dateTo,int referenceTypeId)
        {
            var userId = GetUserIdFromToken();
            var jobCardTransportation = await mediator.Send(new CostJobLineItemsEventQuery((int)userId, jobCardCode, bookingRef, batchNo, customerId, vendorId, clientRef, dateFrom, dateTo, referenceTypeId));
            return new JsonResult(jobCardTransportation);
        }

        [HttpGet]
        [Route("GetJobTransportationById")]
        public async Task<JsonResult> GetJobTransportationById(int id)
        {
            var userId = GetUserIdFromToken();
            var jobCardTransportation = await mediator.Send(new JobCardTransportationEventQuery((int)userId, id));
            return new JsonResult(jobCardTransportation);
        }

        [HttpGet]
        [Route("GetJobHotelsByJobCardId")]
        public async Task<JsonResult> GetJobHotelsByJobCardId(int id)
        {
            var userId = GetUserIdFromToken();
            var jobCardHotel = await mediator.Send(new JobCardHotelsEventQuery((int)userId, id));
                return new JsonResult(jobCardHotel);
        }

        [HttpGet]
        [Route("GetJobHotelById")]
        public async Task<JsonResult> GetJobHotelById(int id)
        {
            var userId = GetUserIdFromToken();
            var jobCardHotel = await mediator.Send(new JobCardHotelEventQuery((int)userId, id));
            return new JsonResult(jobCardHotel);
        }

        [HttpGet]
        [Route("GetJobVisasByJobCardId")]
        public async Task<JsonResult> GetJobVisasByJobCardId(int id)
        {
            var userId = GetUserIdFromToken();
            var jobCardVisa = await mediator.Send(new JobCardVisasEventQuery((int)userId, id));
            return new JsonResult(jobCardVisa);
        }

        [HttpGet]
        [Route("GetJobVisasById")]
        public async Task<JsonResult> GetJobVisasById(int id)
        {
            var userId = GetUserIdFromToken();
            var jobCardVisa = await mediator.Send(new JobCardVisaEventQuery((int)userId, id));
            return new JsonResult(jobCardVisa);
        }

        [HttpGet]
        [Route("GetJobMiscellaneaByJobCardId")]
        public async Task<JsonResult> GetJobMiscellaneaByJobCardId(int id)
        {
            var userId = GetUserIdFromToken();
            var jobCardMiscellaneous = await mediator.Send(new JobCardMiscellaneaEventQuery((int)userId, id));
            return new JsonResult(jobCardMiscellaneous);
        }


        [HttpGet]
        [Route("GetJobMiscellaneousById")]
        public async Task<JsonResult> GetJobMiscellaneousById(int id)
        {
            var userId = GetUserIdFromToken();
            var jobCardMiscellaneous = await mediator.Send(new JobCardMiscellaneousEventQuery((int)userId, id));
            return new JsonResult(jobCardMiscellaneous);
        }

        [HttpGet]
        [Route("GetJobFinancRreceiptAndPaymentsJobCardById")]
        public async Task<JsonResult> GetJobFinancRreceiptAndPaymentsJobCardById(int id)
        {
            var userId = GetUserIdFromToken();
            var jobCardFinancRreceiptAndPayment = await mediator.Send(new JobCardFinancRreceiptAndPaymentEventQuery((int)userId, id));
            return new JsonResult(jobCardFinancRreceiptAndPayment);
        }


        [HttpPost]
        [Route("SaveTransportation")]
        public async Task<JsonResult> SaveTransportation(SaveJobCardTransportationRequest saveJobCardTransportationRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new SaveJobCardTransportationCommand(saveJobCardTransportationRequest.Id, saveJobCardTransportationRequest.JobCardId, saveJobCardTransportationRequest.CustomerRef, saveJobCardTransportationRequest.PaxName, saveJobCardTransportationRequest.Adults, saveJobCardTransportationRequest.Children, saveJobCardTransportationRequest.Infants, saveJobCardTransportationRequest.VehicleType, saveJobCardTransportationRequest.PickupLocation, saveJobCardTransportationRequest.PickupTime, saveJobCardTransportationRequest.DropoffLocation, saveJobCardTransportationRequest.FlightNo, saveJobCardTransportationRequest.FlightTime, saveJobCardTransportationRequest.IsVatIncludedCost, saveJobCardTransportationRequest.IsVatIncludedSell, saveJobCardTransportationRequest.CostBaseAmount, saveJobCardTransportationRequest.CostTaxAmount, saveJobCardTransportationRequest.SellBaseAmount, saveJobCardTransportationRequest.SellTaxAmount, saveJobCardTransportationRequest.Parking, saveJobCardTransportationRequest.ParkingTaxAmount, saveJobCardTransportationRequest.Water, saveJobCardTransportationRequest.WaterTaxAmount, saveJobCardTransportationRequest.Extras, saveJobCardTransportationRequest.ExtrasTaxAmount, saveJobCardTransportationRequest.ParkingSell, saveJobCardTransportationRequest.ParkingTaxAmountSell, saveJobCardTransportationRequest.WaterSell, saveJobCardTransportationRequest.WaterTaxAmountSell, saveJobCardTransportationRequest.ExtrasSell, saveJobCardTransportationRequest.ExtrasTaxAmountSell, saveJobCardTransportationRequest.Remarks, (int)userId));

            return new JsonResult(result);
        }

        [HttpPost]
        [Route("RemoveTransportation")]
        public async Task<JsonResult> RemoveTransportation(RemoveRequest removeRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new RemoveTransportationCommand(removeRequest.Id, (int)userId));

            return new JsonResult(result);
        }

        [HttpPost]
        [Route("SaveHotel")]
        public async Task<JsonResult> SaveHotel(SaveJobCardHotelRequest saveJobCardHotelRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new SaveJobCardHotelCommand(saveJobCardHotelRequest.Id, saveJobCardHotelRequest.JobCardId, saveJobCardHotelRequest.PaxName, saveJobCardHotelRequest.Adults, saveJobCardHotelRequest.Children, saveJobCardHotelRequest.Infants, saveJobCardHotelRequest.VendorId, saveJobCardHotelRequest.HotelName, saveJobCardHotelRequest.CheckIn, saveJobCardHotelRequest.CheckOut, saveJobCardHotelRequest.IsVatIncludedCost, saveJobCardHotelRequest.IsVatIncludedSell, saveJobCardHotelRequest.CostBaseAmount, saveJobCardHotelRequest.CostTaxAmount, saveJobCardHotelRequest.SellBaseAmount, saveJobCardHotelRequest.SellTaxAmount, saveJobCardHotelRequest.Remarks, saveJobCardHotelRequest.HotelConfirmation, saveJobCardHotelRequest.RoomType, saveJobCardHotelRequest.HotelAddress1, saveJobCardHotelRequest.HotelAddress2,(int)userId));

            return new JsonResult(result);
        }

        [HttpPost]
        [Route("RemoveHotel")]
        public async Task<JsonResult> RemoveHotel(RemoveRequest removeRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new RemoveHotelCommand(removeRequest.Id, (int)userId));

            return new JsonResult(result);
        }

        [HttpPost]
        [Route("SaveVisa")]
        public async Task<JsonResult> SaveVisa(SaveJobCardVisaRequest saveJobCardVisaRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new SaveJobCardVisaCommand(saveJobCardVisaRequest.Id, saveJobCardVisaRequest.JobCardId, saveJobCardVisaRequest.PaxName, saveJobCardVisaRequest.PassportNo, saveJobCardVisaRequest.VisaTypeId, saveJobCardVisaRequest.VendorId, saveJobCardVisaRequest.IsVatIncludedCost, saveJobCardVisaRequest.IsVatIncludedSell, saveJobCardVisaRequest.CostBaseAmount, saveJobCardVisaRequest.CostTaxAmount, saveJobCardVisaRequest.SellBaseAmount, saveJobCardVisaRequest.SellTaxAmount, saveJobCardVisaRequest.Remarks, saveJobCardVisaRequest.Nationality, (int)userId));

            return new JsonResult(result);
        }

        [HttpPost]
        [Route("RemoveVisa")]
        public async Task<JsonResult> RemoveVisa(RemoveRequest removeRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new RemoveVisalCommand(removeRequest.Id, (int)userId));

            return new JsonResult(result);
        }
        [HttpPost]
        [Route("SaveMiscellaneous")]
        public async Task<JsonResult> SaveMiscellaneous(SaveJobCardMiscellaneousRequest saveJobCardMiscellaneousRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new SaveJobMiscellaneousCommand(saveJobCardMiscellaneousRequest.Id, saveJobCardMiscellaneousRequest.JobCardId, saveJobCardMiscellaneousRequest.VendorId, saveJobCardMiscellaneousRequest.PaxName, saveJobCardMiscellaneousRequest.PaxNumber, saveJobCardMiscellaneousRequest.Description, saveJobCardMiscellaneousRequest.Remarks, saveJobCardMiscellaneousRequest.MisDate, saveJobCardMiscellaneousRequest.IsVatIncludedCost, saveJobCardMiscellaneousRequest.IsVatIncludedSell, saveJobCardMiscellaneousRequest.CostBaseAmount, saveJobCardMiscellaneousRequest.CostTaxAmount, saveJobCardMiscellaneousRequest.SellBaseAmount, saveJobCardMiscellaneousRequest.SellTaxAmount, saveJobCardMiscellaneousRequest.IsFinance, (int)userId));

            return new JsonResult(result);
        } 

        [HttpPost]
        [Route("RemoveMiscellaneous")]
        public async Task<JsonResult> RemoveMiscellaneous(RemoveRequest removeRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new RemoveMiscellaneousCommand(removeRequest.Id, (int)userId));

            return new JsonResult(result);
        }

        [HttpPost]
        [Route("CloseJobCard")]
        public async Task<JsonResult> CloseJobCard(Request.CloseJobCardRequest closeJobCardRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new CloseJobCardCommand(closeJobCardRequest.Id, (int)userId));

            return new JsonResult(result);
        }

        [HttpPost]
        [Route("OpenJobCard")]
        public async Task<JsonResult> OpenJobCard(Request.OpenJobCardRequest openJobCardRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new OpenJobCardCommand(openJobCardRequest.Id, (int)userId));

            return new JsonResult(result);
        }

        [HttpGet]
        [Route("GetReportItems")]
        public async Task<JsonResult> GetReportItems(string? jobCardCode, string? bookingRef, string? batchNo, string? customerId, string? vendorId, string? clientRef, DateTime? dateFrom, DateTime? dateTo,int? reportTypeId)
        {
            var userId = GetUserIdFromToken();
            var customerIds = customerId == null ? new List<int>() : customerId.Split(',').Select(Int32.Parse).ToList();
            var vendorIds = vendorId == null ? new List<int>() : vendorId.Split(',').Select(Int32.Parse).ToList();

            var reportItems = await mediator.Send(new ReportEventQuery((int)userId, jobCardCode, bookingRef, batchNo, customerIds, vendorIds, clientRef, dateFrom, dateTo,reportTypeId));
            return new JsonResult(reportItems);
        }

        [HttpGet]
        [Route("GetHistoryReportItems")]
        public async Task<JsonResult> GetHistoryReportItems(string? jobCardCode, string? rUserId, DateTime? dateFrom, DateTime? dateTo)
        {
            var userId = GetUserIdFromToken();
            var rUserIds = rUserId == null ? new List<int>() : rUserId.Split(',').Select(Int32.Parse).ToList();
            var reportItems = await mediator.Send(new HistoryReportEventQuery((int)userId, rUserIds, jobCardCode, dateFrom, dateTo));
            return new JsonResult(reportItems);
        }

        [HttpGet]
        [Route("GetUnInvoiceReportItems")]
        public async Task<JsonResult> GetUnInvoiceReportItems(string? jobCardCode, string? bookingRef, string? batchNo, string? customerId, string? vendorId, string? clientRef, DateTime? dateFrom, DateTime? dateTo, int? reportTypeId)
        {
            var userId = GetUserIdFromToken();
            var customerIds = customerId == null ? new List<int>() : customerId.Split(',').Select(Int32.Parse).ToList();
            var vendorIds = vendorId == null ? new List<int>() : vendorId.Split(',').Select(Int32.Parse).ToList();
            var reportItems = await mediator.Send(new UnInvoiceReportEventQuery((int)userId, jobCardCode, bookingRef, batchNo, customerIds, vendorIds, clientRef, dateFrom, dateTo, reportTypeId));
            return new JsonResult(reportItems);
        }

        [HttpPost]
        [Route("UpdateCost")]
        public async Task<JsonResult> UpdateCost(CostUpdateRequest costUpdateRequest)
        {
            var userId = GetUserIdFromToken();
            List<RequestSaveEnvelop> requestSaveEnvelops= new List<RequestSaveEnvelop>();
            foreach (var costRequest in costUpdateRequest.UpdatedRecords)
            {
                var result = await mediator.Send(new UpdateCostCommand(costRequest.Id, costRequest.IsVatIncludedCost, costRequest.IsVatIncludedSell, costRequest.CostBaseAmount, costRequest.CostTaxAmount, costRequest.SellBaseAmount, costRequest.SellTaxAmount, costRequest.Parking, costRequest.Water, costRequest.Extras, costRequest.ExtrasTaxAmount, costRequest.parkingSell, costRequest.waterSell, costRequest.extrasSell, costRequest.extrasTaxAmountSell,(int)userId));
                requestSaveEnvelops.Add(result);
            }

            if (requestSaveEnvelops.Any(i => !i.Created))
            {
                return new JsonResult(requestSaveEnvelops.FirstOrDefault(i => !i.Created));
            }
            return new JsonResult(requestSaveEnvelops.FirstOrDefault());
        }
    }
}

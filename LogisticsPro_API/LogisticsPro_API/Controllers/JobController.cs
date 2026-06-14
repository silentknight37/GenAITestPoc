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
    // Routes follow BA Section 8.6 (resource-oriented) and 8.5 (HTTP method usage):
    // GET = retrieval, POST = create, PUT = update, DELETE = removal,
    // POST .../{id:int}/{action} = controlled state transitions.
    [ApiController]
    [Authorize]
    public class JobController : BaseController
    {
        private readonly IMediator mediator;
        public JobController(IMediator mediator, JwtService jwtservice) : base(jwtservice)
        {
            this.mediator = mediator;
        }

        // ----- Job cards -----

        [HttpGet("~/api/jobcards")]
        public async Task<JsonResult> GetJobs(bool isFirstLoad,string? jobCardCode,string? jobCardDescription, string? customerId, DateTime? effectiveDateFrom,DateTime? effectiveDateTo, string? statusId)
        {
            var userId = GetUserIdFromToken();
            var customerIds = customerId == null ? new List<int>() : customerId.Split(',').Select(Int32.Parse).ToList();
            var statusIds = statusId == null ? new List<int>() : statusId.Split(',').Select(Int32.Parse).ToList();
            var jobs = await mediator.Send(new JobEventQuery(isFirstLoad,(int)userId, jobCardCode,jobCardDescription, customerIds, effectiveDateFrom,effectiveDateTo, statusIds));
            return new JsonResult(jobs);
        }

        [HttpGet("~/api/jobcards/{id:int}")]
        public async Task<JsonResult> GetJobById(int id)
        {
            var userId = GetUserIdFromToken();
            var job = await mediator.Send(new JobCardEventQuery((int)userId, id));
            return new JsonResult(job);
        }

        [HttpPost("~/api/jobcards")]
        [HttpPut("~/api/jobcards")]
        public async Task<JsonResult> SaveJob(SaveJobRequest saveJobRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new SaveJobCommand(saveJobRequest.Id,saveJobRequest.JobDescription, saveJobRequest.CustomerId, saveJobRequest.CustomerRef, DateTime.Parse(saveJobRequest.EffectiveDate), saveJobRequest.Remarks, (int)userId));

            return new JsonResult(result);
        }

        [HttpDelete("~/api/jobcards/{id:int}")]
        public async Task<JsonResult> RemoveJobCard(int id)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new RemoveJobCardCommand(id, (int)userId));

            return new JsonResult(result);
        }

        [HttpPost("~/api/jobcards/{id:int}/close")]
        public async Task<JsonResult> CloseJobCard(int id)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new CloseJobCardCommand(id, (int)userId));

            return new JsonResult(result);
        }

        [HttpPost("~/api/jobcards/{id:int}/open")]
        public async Task<JsonResult> OpenJobCard(int id)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new OpenJobCardCommand(id, (int)userId));

            return new JsonResult(result);
        }

        [HttpGet("~/api/jobcards/{id:int}/finance")]
        public async Task<JsonResult> GetJobFinancRreceiptAndPaymentsJobCardById(int id)
        {
            var userId = GetUserIdFromToken();
            var jobCardFinancRreceiptAndPayment = await mediator.Send(new JobCardFinancRreceiptAndPaymentEventQuery((int)userId, id));
            return new JsonResult(jobCardFinancRreceiptAndPayment);
        }

        // ----- Transportation services -----

        [HttpGet("~/api/jobcards/{id:int}/transportation-services")]
        public async Task<JsonResult> GetJobTransportationsByJobCardId(int id)
        {
            var userId = GetUserIdFromToken();
            var jobCardTransportation = await mediator.Send(new JobCardTransportationsEventQuery((int)userId, id));
            return new JsonResult(jobCardTransportation);
        }

        [HttpGet("~/api/transportation-services")]
        public async Task<JsonResult> GetJobTransportations(string? jobCardCode, string? bookingRef, string? batchNo, int? customerId, int? vendorId, string? clientRef, DateTime? dateFrom, DateTime? dateTo)
        {
            var userId = GetUserIdFromToken();
            var jobCardTransportation = await mediator.Send(new CostTransportationsEventQuery((int)userId, jobCardCode, bookingRef,batchNo,customerId,vendorId,clientRef,dateFrom,dateTo));
            return new JsonResult(jobCardTransportation);
        }

        [HttpGet("~/api/job-line-items")]
        public async Task<JsonResult> GetJobLineItems(string? jobCardCode, string? bookingRef, string? batchNo, int? customerId, int? vendorId, string? clientRef, DateTime? dateFrom, DateTime? dateTo,int referenceTypeId)
        {
            var userId = GetUserIdFromToken();
            var jobCardTransportation = await mediator.Send(new CostJobLineItemsEventQuery((int)userId, jobCardCode, bookingRef, batchNo, customerId, vendorId, clientRef, dateFrom, dateTo, referenceTypeId));
            return new JsonResult(jobCardTransportation);
        }

        [HttpGet("~/api/transportation-services/{id:int}")]
        public async Task<JsonResult> GetJobTransportationById(int id)
        {
            var userId = GetUserIdFromToken();
            var jobCardTransportation = await mediator.Send(new JobCardTransportationEventQuery((int)userId, id));
            return new JsonResult(jobCardTransportation);
        }

        [HttpPost("~/api/transportation-services")]
        [HttpPut("~/api/transportation-services")]
        public async Task<JsonResult> SaveTransportation(SaveJobCardTransportationRequest saveJobCardTransportationRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new SaveJobCardTransportationCommand(saveJobCardTransportationRequest.Id, saveJobCardTransportationRequest.JobCardId, saveJobCardTransportationRequest.CustomerRef, saveJobCardTransportationRequest.PaxName, saveJobCardTransportationRequest.Adults, saveJobCardTransportationRequest.Children, saveJobCardTransportationRequest.Infants, saveJobCardTransportationRequest.VehicleType, saveJobCardTransportationRequest.PickupLocation, saveJobCardTransportationRequest.PickupTime, saveJobCardTransportationRequest.DropoffLocation, saveJobCardTransportationRequest.FlightNo, saveJobCardTransportationRequest.FlightTime, saveJobCardTransportationRequest.IsVatIncludedCost, saveJobCardTransportationRequest.IsVatIncludedSell, saveJobCardTransportationRequest.CostBaseAmount, saveJobCardTransportationRequest.CostTaxAmount, saveJobCardTransportationRequest.SellBaseAmount, saveJobCardTransportationRequest.SellTaxAmount, saveJobCardTransportationRequest.Parking, saveJobCardTransportationRequest.ParkingTaxAmount, saveJobCardTransportationRequest.Water, saveJobCardTransportationRequest.WaterTaxAmount, saveJobCardTransportationRequest.Extras, saveJobCardTransportationRequest.ExtrasTaxAmount, saveJobCardTransportationRequest.ParkingSell, saveJobCardTransportationRequest.ParkingTaxAmountSell, saveJobCardTransportationRequest.WaterSell, saveJobCardTransportationRequest.WaterTaxAmountSell, saveJobCardTransportationRequest.ExtrasSell, saveJobCardTransportationRequest.ExtrasTaxAmountSell, saveJobCardTransportationRequest.Remarks, (int)userId));

            return new JsonResult(result);
        }

        [HttpDelete("~/api/transportation-services/{id:int}")]
        public async Task<JsonResult> RemoveTransportation(int id)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new RemoveTransportationCommand(id, (int)userId));

            return new JsonResult(result);
        }

        // ----- Hotel services -----

        [HttpGet("~/api/jobcards/{id:int}/hotel-services")]
        public async Task<JsonResult> GetJobHotelsByJobCardId(int id)
        {
            var userId = GetUserIdFromToken();
            var jobCardHotel = await mediator.Send(new JobCardHotelsEventQuery((int)userId, id));
                return new JsonResult(jobCardHotel);
        }

        [HttpGet("~/api/hotel-services/{id:int}")]
        public async Task<JsonResult> GetJobHotelById(int id)
        {
            var userId = GetUserIdFromToken();
            var jobCardHotel = await mediator.Send(new JobCardHotelEventQuery((int)userId, id));
            return new JsonResult(jobCardHotel);
        }

        [HttpPost("~/api/hotel-services")]
        [HttpPut("~/api/hotel-services")]
        public async Task<JsonResult> SaveHotel(SaveJobCardHotelRequest saveJobCardHotelRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new SaveJobCardHotelCommand(saveJobCardHotelRequest.Id, saveJobCardHotelRequest.JobCardId, saveJobCardHotelRequest.PaxName, saveJobCardHotelRequest.Adults, saveJobCardHotelRequest.Children, saveJobCardHotelRequest.Infants, saveJobCardHotelRequest.VendorId, saveJobCardHotelRequest.HotelName, saveJobCardHotelRequest.CheckIn, saveJobCardHotelRequest.CheckOut, saveJobCardHotelRequest.IsVatIncludedCost, saveJobCardHotelRequest.IsVatIncludedSell, saveJobCardHotelRequest.CostBaseAmount, saveJobCardHotelRequest.CostTaxAmount, saveJobCardHotelRequest.SellBaseAmount, saveJobCardHotelRequest.SellTaxAmount, saveJobCardHotelRequest.Remarks, saveJobCardHotelRequest.HotelConfirmation, saveJobCardHotelRequest.RoomType, saveJobCardHotelRequest.HotelAddress1, saveJobCardHotelRequest.HotelAddress2,(int)userId));

            return new JsonResult(result);
        }

        [HttpDelete("~/api/hotel-services/{id:int}")]
        public async Task<JsonResult> RemoveHotel(int id)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new RemoveHotelCommand(id, (int)userId));

            return new JsonResult(result);
        }

        // ----- Visa services -----

        [HttpGet("~/api/jobcards/{id:int}/visa-services")]
        public async Task<JsonResult> GetJobVisasByJobCardId(int id)
        {
            var userId = GetUserIdFromToken();
            var jobCardVisa = await mediator.Send(new JobCardVisasEventQuery((int)userId, id));
            return new JsonResult(jobCardVisa);
        }

        [HttpGet("~/api/visa-services/{id:int}")]
        public async Task<JsonResult> GetJobVisasById(int id)
        {
            var userId = GetUserIdFromToken();
            var jobCardVisa = await mediator.Send(new JobCardVisaEventQuery((int)userId, id));
            return new JsonResult(jobCardVisa);
        }

        [HttpPost("~/api/visa-services")]
        [HttpPut("~/api/visa-services")]
        public async Task<JsonResult> SaveVisa(SaveJobCardVisaRequest saveJobCardVisaRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new SaveJobCardVisaCommand(saveJobCardVisaRequest.Id, saveJobCardVisaRequest.JobCardId, saveJobCardVisaRequest.PaxName, saveJobCardVisaRequest.PassportNo, saveJobCardVisaRequest.VisaTypeId, saveJobCardVisaRequest.VendorId, saveJobCardVisaRequest.IsVatIncludedCost, saveJobCardVisaRequest.IsVatIncludedSell, saveJobCardVisaRequest.CostBaseAmount, saveJobCardVisaRequest.CostTaxAmount, saveJobCardVisaRequest.SellBaseAmount, saveJobCardVisaRequest.SellTaxAmount, saveJobCardVisaRequest.Remarks, saveJobCardVisaRequest.Nationality, (int)userId));

            return new JsonResult(result);
        }

        [HttpDelete("~/api/visa-services/{id:int}")]
        public async Task<JsonResult> RemoveVisa(int id)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new RemoveVisalCommand(id, (int)userId));

            return new JsonResult(result);
        }

        // ----- Miscellaneous services -----

        [HttpGet("~/api/jobcards/{id:int}/miscellaneous-services")]
        public async Task<JsonResult> GetJobMiscellaneaByJobCardId(int id)
        {
            var userId = GetUserIdFromToken();
            var jobCardMiscellaneous = await mediator.Send(new JobCardMiscellaneaEventQuery((int)userId, id));
            return new JsonResult(jobCardMiscellaneous);
        }

        [HttpGet("~/api/miscellaneous-services/{id:int}")]
        public async Task<JsonResult> GetJobMiscellaneousById(int id)
        {
            var userId = GetUserIdFromToken();
            var jobCardMiscellaneous = await mediator.Send(new JobCardMiscellaneousEventQuery((int)userId, id));
            return new JsonResult(jobCardMiscellaneous);
        }

        [HttpPost("~/api/miscellaneous-services")]
        [HttpPut("~/api/miscellaneous-services")]
        public async Task<JsonResult> SaveMiscellaneous(SaveJobCardMiscellaneousRequest saveJobCardMiscellaneousRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new SaveJobMiscellaneousCommand(saveJobCardMiscellaneousRequest.Id, saveJobCardMiscellaneousRequest.JobCardId, saveJobCardMiscellaneousRequest.VendorId, saveJobCardMiscellaneousRequest.PaxName, saveJobCardMiscellaneousRequest.PaxNumber, saveJobCardMiscellaneousRequest.Description, saveJobCardMiscellaneousRequest.Remarks, saveJobCardMiscellaneousRequest.MisDate, saveJobCardMiscellaneousRequest.IsVatIncludedCost, saveJobCardMiscellaneousRequest.IsVatIncludedSell, saveJobCardMiscellaneousRequest.CostBaseAmount, saveJobCardMiscellaneousRequest.CostTaxAmount, saveJobCardMiscellaneousRequest.SellBaseAmount, saveJobCardMiscellaneousRequest.SellTaxAmount, saveJobCardMiscellaneousRequest.IsFinance, (int)userId));

            return new JsonResult(result);
        }

        [HttpDelete("~/api/miscellaneous-services/{id:int}")]
        public async Task<JsonResult> RemoveMiscellaneous(int id)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new RemoveMiscellaneousCommand(id, (int)userId));

            return new JsonResult(result);
        }

        // ----- Cost management -----

        [HttpPut("~/api/service-costs")]
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

        // ----- Reports -----

        [HttpGet("~/api/reports/operational")]
        public async Task<JsonResult> GetReportItems(string? jobCardCode, string? bookingRef, string? batchNo, string? customerId, string? vendorId, string? clientRef, DateTime? dateFrom, DateTime? dateTo,int? reportTypeId)
        {
            var userId = GetUserIdFromToken();
            var customerIds = customerId == null ? new List<int>() : customerId.Split(',').Select(Int32.Parse).ToList();
            var vendorIds = vendorId == null ? new List<int>() : vendorId.Split(',').Select(Int32.Parse).ToList();

            var reportItems = await mediator.Send(new ReportEventQuery((int)userId, jobCardCode, bookingRef, batchNo, customerIds, vendorIds, clientRef, dateFrom, dateTo,reportTypeId));
            return new JsonResult(reportItems);
        }

        [HttpGet("~/api/reports/history")]
        public async Task<JsonResult> GetHistoryReportItems(string? jobCardCode, string? rUserId, DateTime? dateFrom, DateTime? dateTo)
        {
            var userId = GetUserIdFromToken();
            var rUserIds = rUserId == null ? new List<int>() : rUserId.Split(',').Select(Int32.Parse).ToList();
            var reportItems = await mediator.Send(new HistoryReportEventQuery((int)userId, rUserIds, jobCardCode, dateFrom, dateTo));
            return new JsonResult(reportItems);
        }

        [HttpGet("~/api/reports/uninvoiced")]
        public async Task<JsonResult> GetUnInvoiceReportItems(string? jobCardCode, string? bookingRef, string? batchNo, string? customerId, string? vendorId, string? clientRef, DateTime? dateFrom, DateTime? dateTo, int? reportTypeId)
        {
            var userId = GetUserIdFromToken();
            var customerIds = customerId == null ? new List<int>() : customerId.Split(',').Select(Int32.Parse).ToList();
            var vendorIds = vendorId == null ? new List<int>() : vendorId.Split(',').Select(Int32.Parse).ToList();
            var reportItems = await mediator.Send(new UnInvoiceReportEventQuery((int)userId, jobCardCode, bookingRef, batchNo, customerIds, vendorIds, clientRef, dateFrom, dateTo, reportTypeId));
            return new JsonResult(reportItems);
        }
    }
}
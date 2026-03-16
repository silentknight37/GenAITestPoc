using LogisticsPro_API.Request;
using LogisticsPro_Common.DTO;
using LogisticsPro_Manager.Command;
using LogisticsPro_Manager.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace LogisticsPro_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LogisticsController : BaseController
    {
        private readonly IMediator mediator;
        public LogisticsController(IMediator mediator, JwtService jwtservice) : base(jwtservice)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("GetBatchItems")]
        public async Task<JsonResult> GetBatchItems(DateTime? batchDate)
        {
            var userId = GetUserIdFromToken();
            var jobs = await mediator.Send(new BatchItemEventQuery((int)userId, batchDate));
            return new JsonResult(jobs);
        }

        [HttpPost]
        [Route("SavetBatchItems")]
        public async Task<JsonResult> SavetBatchItems(SaveBatchItemRequest saveBatchItemRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new SaveBatchItemCommand(saveBatchItemRequest.Ids, saveBatchItemRequest.VendorId, saveBatchItemRequest.BatchDate,(int)userId));

            return new JsonResult(result);
        }

        [HttpGet]
        [Route("GetBatches")]
        public async Task<JsonResult> GetBatches(bool isFirstLoad,string? batchCode, string? vendorId, DateTime? batchDateFrom, DateTime? batchDateTo,string? jobCardNumber)
        {
            var userId = GetUserIdFromToken();

            var vendorIds= vendorId == null ? new List<int>() : vendorId.Split(',').Select(Int32.Parse).ToList();
           

            var batches = await mediator.Send(new BatchEventQuery(isFirstLoad,(int)userId, batchCode, vendorIds, batchDateFrom, batchDateTo, jobCardNumber));
            return new JsonResult(batches);
        }

        [HttpPost]
        [Route("RemoveBatchItemFromList")]
        public async Task<JsonResult> RemoveBatchItemFromList(RemoveRequest removeRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new RemoveBatchItemCommand(removeRequest.Id,(int)userId));

            return new JsonResult(result);
        }

        [HttpGet]
        [Route("GetBatchItemsByVendorId")]
        public async Task<JsonResult> GetBatchItemsByVendorId(int vendorId, DateTime? fromDate, DateTime? toDate,string? jobCardNumber)
        {
            var userId = GetUserIdFromToken();
            var jobs = await mediator.Send(new PaymentVoucherGenerateItemQuery((int)userId, vendorId,fromDate,toDate, jobCardNumber));
            return new JsonResult(jobs);
        }

        [HttpPost]
        [Route("SavePaymentVouchers")]
        public async Task<JsonResult> SavePaymentVouchers(SavePaymentVoucherRequest savePaymentVoucherRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new SavePaymentVoucherCommand(savePaymentVoucherRequest.TransportationIds, savePaymentVoucherRequest.HotelIds, savePaymentVoucherRequest.VisaIds, savePaymentVoucherRequest.MiscellaneousIds, savePaymentVoucherRequest.VendorId, savePaymentVoucherRequest.PaymentVoucherDate, savePaymentVoucherRequest.PaymentVoucherAmount, savePaymentVoucherRequest.Invoice, savePaymentVoucherRequest.Remarks,(int)userId));

            return new JsonResult(result);
        }

        [HttpGet]
        [Route("GetPaymentVouchers")]
        public async Task<JsonResult> GetPaymentVouchers(bool isFirstLoad,string? paymentVoucherCode,string? invoiceNo, string? vendorId, DateTime? paymentVoucherDateFrom, DateTime? paymentVoucherDateTo,string? jobCardNumber)
        {
            var userId = GetUserIdFromToken();
            var vendorIds = vendorId == null ? new List<int>() : vendorId.Split(',').Select(Int32.Parse).ToList();
            var batches = await mediator.Send(new PaymentVoucherEventQuery(isFirstLoad,(int)userId, paymentVoucherCode, invoiceNo, vendorIds, paymentVoucherDateFrom, paymentVoucherDateTo,jobCardNumber));
            return new JsonResult(batches);
        }

        [HttpGet]
        [Route("GetPaymentVoucherById")]
        public async Task<JsonResult> GetPaymentVoucherById(int id)
        {
            var userId = GetUserIdFromToken();
            var batches = await mediator.Send(new PaymentVoucherByIdEventQuery((int)userId, id));
            return new JsonResult(batches);
        }

        [HttpPost]
        [Route("RemovePaymentVoucherItemFromList")]
        public async Task<JsonResult> RemovePaymentVoucherItemFromList(PaymentVoucherRemoveRequest paymentVoucherRemoveRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new RemovePaymentVoucherItemCommand(paymentVoucherRemoveRequest.Id, paymentVoucherRemoveRequest.Type,(int)userId));

            return new JsonResult(result);
        }

        [HttpGet]
        [Route("GetJobCardItemsByCustomerId")]
        public async Task<JsonResult> GetJobCardItemsByCustomerId(int customerId, DateTime? fromDate, DateTime? toDate,string? jobCardNumber)
        {
            var userId = GetUserIdFromToken();
            var jobs = await mediator.Send(new JobCardItemByCustomerIdEventQuery((int)userId, customerId, fromDate, toDate, jobCardNumber));
            return new JsonResult(jobs);
        }

        [HttpPost]
        [Route("SaveInvoice")]
        public async Task<JsonResult> SaveInvoice(SaveInvoiceRequest saveInvoiceRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new SaveInvoiceCommand(saveInvoiceRequest.TransportationIds, saveInvoiceRequest.HotelIds, saveInvoiceRequest.VisaIds, saveInvoiceRequest.MiscellaneousIds, saveInvoiceRequest.CustomerId, saveInvoiceRequest.InvoiceDate, saveInvoiceRequest.InvoiceDueDate, saveInvoiceRequest.InvoiceAmount, saveInvoiceRequest.Remarks, saveInvoiceRequest.TransportDescription, saveInvoiceRequest.HotelDescription, saveInvoiceRequest.VisaDescription, saveInvoiceRequest.MiscellaneousDescription, saveInvoiceRequest.ProformaInvoices, (int)userId));

            return new JsonResult(result);
        }

        [HttpGet]
        [Route("GetInvoices")]
        public async Task<JsonResult> GetInvoices(bool isFirstLoad,string? invoiceCode, string? customerId, DateTime? invoiceDateFrom, DateTime? invoiceDateTo, DateTime? invoiceDueDateFrom, DateTime? invoiceDueDateTo, string? statusId,string? jobCardNumber)
        {
            var userId = GetUserIdFromToken();
            var customerIds = customerId == null ? new List<int>() : customerId.Split(',').Select(Int32.Parse).ToList();
            var statusIds = statusId == null ? new List<int>() : statusId.Split(',').Select(Int32.Parse).ToList();
            var batches = await mediator.Send(new InvoiceEventQuery(isFirstLoad,(int)userId, invoiceCode, customerIds, invoiceDateFrom, invoiceDateTo, invoiceDueDateFrom, invoiceDueDateTo, statusIds, jobCardNumber));
            return new JsonResult(batches);
        }

        [HttpPost]
        [Route("VoidInvoice")]
        public async Task<JsonResult> VoidInvoice(RemoveRequest removeRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new VoidInvoiceCommand(removeRequest.Id,(int)userId));

            return new JsonResult(result);
        }

        [HttpPost]
        [Route("SaveProformaInvoice")]
        public async Task<JsonResult> SaveProformaInvoice(SaveProformaInvoiceRequest saveProformaInvoiceRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new SaveProformaInvoiceCommand(saveProformaInvoiceRequest.CustomerId, saveProformaInvoiceRequest.InvoiceDate, saveProformaInvoiceRequest.InvoiceDueDate, saveProformaInvoiceRequest.InvoiceAmount, saveProformaInvoiceRequest.JobCardId, saveProformaInvoiceRequest.Description,(int)userId));

            return new JsonResult(result);
        }

        [HttpGet]
        [Route("GetProformaInvoices")]
        public async Task<JsonResult> GetProformaInvoices(bool isFirstLoad,string? invoiceCode, string? customerId, DateTime? invoiceDateFrom, DateTime? invoiceDateTo, DateTime? invoiceDueDateFrom, DateTime? invoiceDueDateTo, string? statusId, string? jobCardNumber)
        {
            var userId = GetUserIdFromToken();
            var customerIds = customerId == null ? new List<int>() : customerId.Split(',').Select(Int32.Parse).ToList();
            var statusIds = statusId == null ? new List<int>() : statusId.Split(',').Select(Int32.Parse).ToList();

            var batches = await mediator.Send(new ProformaInvoiceEventQuery(isFirstLoad,(int)userId, invoiceCode, customerIds, invoiceDateFrom, invoiceDateTo, invoiceDueDateFrom, invoiceDueDateTo, statusIds,jobCardNumber));
            return new JsonResult(batches);
        }

        [HttpPost]
        [Route("VoidProformaInvoice")]
        public async Task<JsonResult> VoidProformaInvoice(RemoveRequest removeRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new VoidProformaInvoiceCommand(removeRequest.Id,(int)userId));

            return new JsonResult(result);
        }

        [HttpGet]
        [Route("GetProformaInvoice")]
        public async Task<JsonResult> GetProformaInvoice(int id)
        {
            var userId = GetUserIdFromToken();
            var invoice = await mediator.Send(new ProformaInvoiceByIdEventQuery((int)userId, id));
            return new JsonResult(invoice);
        }

        [HttpGet]
        [Route("GetProformaInvoiceReceipts")]
        public async Task<JsonResult> GetProformaInvoiceReceipts(int id)
        {
            var userId = GetUserIdFromToken();
            var invoice = await mediator.Send(new ProformaInvoiceReceiptsByInvoiceIdEventQuery((int)userId, id));
            return new JsonResult(invoice);
        }


        [HttpPost]
        [Route("SaveProformaInvoiceReceipt")]
        public async Task<JsonResult> SaveProformaInvoiceReceipt(SaveProformaInvoiceReceiptRequest saveProformaInvoiceReceiptRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new SaveProformaInvoiceReceiptCommand(saveProformaInvoiceReceiptRequest.Id, saveProformaInvoiceReceiptRequest.ReceiptDate, saveProformaInvoiceReceiptRequest.Amount, saveProformaInvoiceReceiptRequest.PaymentMethod, saveProformaInvoiceReceiptRequest.ProformaInvoiceId, saveProformaInvoiceReceiptRequest.JobCardId, saveProformaInvoiceReceiptRequest.Remark,(int)userId));

            return new JsonResult(result);
        }

        [HttpPost]
        [Route("RemoveProformaInvoiceReceipt")]
        public async Task<JsonResult> RemoveProformaInvoiceReceipt(RemoveRequest removeRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new RemoveProformaInvoiceReceiptCommand(removeRequest.Id, (int)userId));

            return new JsonResult(result);
        }


        [HttpGet]
        [Route("GetInvoice")]
        public async Task<JsonResult> GetInvoice(int id)
        {
            var userId = GetUserIdFromToken();
            var invoice = await mediator.Send(new InvoiceByIdEventQuery((int)userId, id));
            return new JsonResult(invoice);
        }

        [HttpGet]
        [Route("GetReceipts")]
        public async Task<JsonResult> GetReceipts(int id)
        {
            var userId = GetUserIdFromToken();
            var receipts = await mediator.Send(new ReceiptsByInvoiceIdEventQuery((int)userId, id));
            return new JsonResult(receipts);
        }


        [HttpPost]
        [Route("SaveReceipt")]
        public async Task<JsonResult> SaveReceipt(SaveReceiptRequest saveReceiptRequest)
        {
            var userId = GetUserIdFromToken();
            List<LogisticsPro_Manager.Command.UpdateRecords> updateRecords = new List<LogisticsPro_Manager.Command.UpdateRecords> ();
            saveReceiptRequest.UpdateRecords.ForEach(i => updateRecords.Add(new LogisticsPro_Manager.Command.UpdateRecords
            {
                Id = i.Id,
                ServiceType=i.Type,
                AllocatedAmount=i.AllocatedAmount
            }));

            var result = await mediator.Send(new SaveReceiptCommand(saveReceiptRequest.Id, saveReceiptRequest.ReceiptDate, saveReceiptRequest.Amount, saveReceiptRequest.PaymentMethod, saveReceiptRequest.InvoiceId, saveReceiptRequest.Remark, updateRecords, (int)userId));

            return new JsonResult(result);
        }

        [HttpPost]
        [Route("RemoveReceipt")]
        public async Task<JsonResult> RemoveReceipt(RemoveRequest removeRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new RemoveProformaInvoiceReceiptCommand(removeRequest.Id, (int)userId));

            return new JsonResult(result);
        }

        [HttpPost]
        [Route("VoidReceipt")]
        public async Task<JsonResult> VoidReceipt(VoidRequest voidRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new VoidInvoiceReceiptCommand(voidRequest.Id, (int)userId));

            return new JsonResult(result);
        }

        [HttpGet]
        [Route("GetPnL")]
        public async Task<JsonResult> GetPnL(string? customerId, DateTime? dateFrom, DateTime? dateTo, string? jobCardNumber)
        {
            var userId = GetUserIdFromToken();
            var customerIds = customerId == null ? new List<int>() : customerId.Split(',').Select(Int32.Parse).ToList();
            var batches = await mediator.Send(new PnLEventQuery((int)userId, customerIds, dateFrom, dateTo,jobCardNumber));
            return new JsonResult(batches);
        }
    }
}

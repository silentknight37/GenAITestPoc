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
    // Routes follow BA Section 8.6 (resource-oriented) and 8.5 (HTTP method usage):
    // GET = retrieval, POST = create, PUT = update, DELETE = removal,
    // POST .../{id:int}/void = controlled, non-destructive financial reversal (BA 7.9.4).
    [ApiController]
    [Authorize]
    public class LogisticsController : BaseController
    {
        private readonly IMediator mediator;
        public LogisticsController(IMediator mediator, JwtService jwtservice) : base(jwtservice)
        {
            this.mediator = mediator;
        }

        // ----- Batches -----

        [HttpGet("~/api/batch-eligible-items")]
        public async Task<JsonResult> GetBatchItems(DateTime? batchDate)
        {
            var userId = GetUserIdFromToken();
            var jobs = await mediator.Send(new BatchItemEventQuery((int)userId, batchDate));
            return new JsonResult(jobs);
        }

        [HttpPost("~/api/batches")]
        public async Task<JsonResult> SavetBatchItems(SaveBatchItemRequest saveBatchItemRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new SaveBatchItemCommand(saveBatchItemRequest.Ids, saveBatchItemRequest.VendorId, saveBatchItemRequest.BatchDate,(int)userId));

            return new JsonResult(result);
        }

        [HttpGet("~/api/batches")]
        public async Task<JsonResult> GetBatches(bool isFirstLoad,string? batchCode, string? vendorId, DateTime? batchDateFrom, DateTime? batchDateTo,string? jobCardNumber)
        {
            var userId = GetUserIdFromToken();

            var vendorIds= vendorId == null ? new List<int>() : vendorId.Split(',').Select(Int32.Parse).ToList();


            var batches = await mediator.Send(new BatchEventQuery(isFirstLoad,(int)userId, batchCode, vendorIds, batchDateFrom, batchDateTo, jobCardNumber));
            return new JsonResult(batches);
        }

        [HttpDelete("~/api/batch-items/{id:int}")]
        public async Task<JsonResult> RemoveBatchItemFromList(int id)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new RemoveBatchItemCommand(id,(int)userId));

            return new JsonResult(result);
        }

        // ----- Payment vouchers -----

        [HttpGet("~/api/vendors/{vendorId:int}/payable-items")]
        public async Task<JsonResult> GetBatchItemsByVendorId(int vendorId, DateTime? fromDate, DateTime? toDate,string? jobCardNumber)
        {
            var userId = GetUserIdFromToken();
            var jobs = await mediator.Send(new PaymentVoucherGenerateItemQuery((int)userId, vendorId,fromDate,toDate, jobCardNumber));
            return new JsonResult(jobs);
        }

        [HttpPost("~/api/payment-vouchers")]
        public async Task<JsonResult> SavePaymentVouchers(SavePaymentVoucherRequest savePaymentVoucherRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new SavePaymentVoucherCommand(savePaymentVoucherRequest.TransportationIds, savePaymentVoucherRequest.HotelIds, savePaymentVoucherRequest.VisaIds, savePaymentVoucherRequest.MiscellaneousIds, savePaymentVoucherRequest.VendorId, savePaymentVoucherRequest.PaymentVoucherDate, savePaymentVoucherRequest.PaymentVoucherAmount, savePaymentVoucherRequest.Invoice, savePaymentVoucherRequest.Remarks,(int)userId));

            return new JsonResult(result);
        }

        [HttpGet("~/api/payment-vouchers")]
        public async Task<JsonResult> GetPaymentVouchers(bool isFirstLoad,string? paymentVoucherCode,string? invoiceNo, string? vendorId, DateTime? paymentVoucherDateFrom, DateTime? paymentVoucherDateTo,string? jobCardNumber)
        {
            var userId = GetUserIdFromToken();
            var vendorIds = vendorId == null ? new List<int>() : vendorId.Split(',').Select(Int32.Parse).ToList();
            var batches = await mediator.Send(new PaymentVoucherEventQuery(isFirstLoad,(int)userId, paymentVoucherCode, invoiceNo, vendorIds, paymentVoucherDateFrom, paymentVoucherDateTo,jobCardNumber));
            return new JsonResult(batches);
        }

        [HttpGet("~/api/payment-vouchers/{id:int}")]
        public async Task<JsonResult> GetPaymentVoucherById(int id)
        {
            var userId = GetUserIdFromToken();
            var batches = await mediator.Send(new PaymentVoucherByIdEventQuery((int)userId, id));
            return new JsonResult(batches);
        }

        [HttpDelete("~/api/payment-voucher-items/{id:int}")]
        public async Task<JsonResult> RemovePaymentVoucherItemFromList(int id, [FromQuery] string type)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new RemovePaymentVoucherItemCommand(id, type,(int)userId));

            return new JsonResult(result);
        }

        // ----- Invoices -----

        [HttpGet("~/api/customers/{customerId:int}/billable-items")]
        public async Task<JsonResult> GetJobCardItemsByCustomerId(int customerId, DateTime? fromDate, DateTime? toDate,string? jobCardNumber)
        {
            var userId = GetUserIdFromToken();
            var jobs = await mediator.Send(new JobCardItemByCustomerIdEventQuery((int)userId, customerId, fromDate, toDate, jobCardNumber));
            return new JsonResult(jobs);
        }

        [HttpPost("~/api/invoices")]
        public async Task<JsonResult> SaveInvoice(SaveInvoiceRequest saveInvoiceRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new SaveInvoiceCommand(saveInvoiceRequest.TransportationIds, saveInvoiceRequest.HotelIds, saveInvoiceRequest.VisaIds, saveInvoiceRequest.MiscellaneousIds, saveInvoiceRequest.CustomerId, saveInvoiceRequest.InvoiceDate, saveInvoiceRequest.InvoiceDueDate, saveInvoiceRequest.InvoiceAmount, saveInvoiceRequest.Remarks, saveInvoiceRequest.TransportDescription, saveInvoiceRequest.HotelDescription, saveInvoiceRequest.VisaDescription, saveInvoiceRequest.MiscellaneousDescription, saveInvoiceRequest.ProformaInvoices, (int)userId));

            return new JsonResult(result);
        }

        [HttpGet("~/api/invoices")]
        public async Task<JsonResult> GetInvoices(bool isFirstLoad,string? invoiceCode, string? customerId, DateTime? invoiceDateFrom, DateTime? invoiceDateTo, DateTime? invoiceDueDateFrom, DateTime? invoiceDueDateTo, string? statusId,string? jobCardNumber)
        {
            var userId = GetUserIdFromToken();
            var customerIds = customerId == null ? new List<int>() : customerId.Split(',').Select(Int32.Parse).ToList();
            var statusIds = statusId == null ? new List<int>() : statusId.Split(',').Select(Int32.Parse).ToList();
            var batches = await mediator.Send(new InvoiceEventQuery(isFirstLoad,(int)userId, invoiceCode, customerIds, invoiceDateFrom, invoiceDateTo, invoiceDueDateFrom, invoiceDueDateTo, statusIds, jobCardNumber));
            return new JsonResult(batches);
        }

        [HttpGet("~/api/invoices/{id:int}")]
        public async Task<JsonResult> GetInvoice(int id)
        {
            var userId = GetUserIdFromToken();
            var invoice = await mediator.Send(new InvoiceByIdEventQuery((int)userId, id));
            return new JsonResult(invoice);
        }

        [HttpPost("~/api/invoices/{id:int}/void")]
        public async Task<JsonResult> VoidInvoice(int id)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new VoidInvoiceCommand(id,(int)userId));

            return new JsonResult(result);
        }

        [HttpGet("~/api/invoices/{id:int}/receipts")]
        public async Task<JsonResult> GetReceipts(int id)
        {
            var userId = GetUserIdFromToken();
            var receipts = await mediator.Send(new ReceiptsByInvoiceIdEventQuery((int)userId, id));
            return new JsonResult(receipts);
        }

        // ----- Proforma invoices -----

        [HttpPost("~/api/proforma-invoices")]
        public async Task<JsonResult> SaveProformaInvoice(SaveProformaInvoiceRequest saveProformaInvoiceRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new SaveProformaInvoiceCommand(saveProformaInvoiceRequest.CustomerId, saveProformaInvoiceRequest.InvoiceDate, saveProformaInvoiceRequest.InvoiceDueDate, saveProformaInvoiceRequest.InvoiceAmount, saveProformaInvoiceRequest.JobCardId, saveProformaInvoiceRequest.Description,(int)userId));

            return new JsonResult(result);
        }

        [HttpGet("~/api/proforma-invoices")]
        public async Task<JsonResult> GetProformaInvoices(bool isFirstLoad,string? invoiceCode, string? customerId, DateTime? invoiceDateFrom, DateTime? invoiceDateTo, DateTime? invoiceDueDateFrom, DateTime? invoiceDueDateTo, string? statusId, string? jobCardNumber)
        {
            var userId = GetUserIdFromToken();
            var customerIds = customerId == null ? new List<int>() : customerId.Split(',').Select(Int32.Parse).ToList();
            var statusIds = statusId == null ? new List<int>() : statusId.Split(',').Select(Int32.Parse).ToList();

            var batches = await mediator.Send(new ProformaInvoiceEventQuery(isFirstLoad,(int)userId, invoiceCode, customerIds, invoiceDateFrom, invoiceDateTo, invoiceDueDateFrom, invoiceDueDateTo, statusIds,jobCardNumber));
            return new JsonResult(batches);
        }

        [HttpGet("~/api/proforma-invoices/{id:int}")]
        public async Task<JsonResult> GetProformaInvoice(int id)
        {
            var userId = GetUserIdFromToken();
            var invoice = await mediator.Send(new ProformaInvoiceByIdEventQuery((int)userId, id));
            return new JsonResult(invoice);
        }

        [HttpPost("~/api/proforma-invoices/{id:int}/void")]
        public async Task<JsonResult> VoidProformaInvoice(int id)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new VoidProformaInvoiceCommand(id,(int)userId));

            return new JsonResult(result);
        }

        [HttpGet("~/api/proforma-invoices/{id:int}/receipts")]
        public async Task<JsonResult> GetProformaInvoiceReceipts(int id)
        {
            var userId = GetUserIdFromToken();
            var invoice = await mediator.Send(new ProformaInvoiceReceiptsByInvoiceIdEventQuery((int)userId, id));
            return new JsonResult(invoice);
        }

        [HttpPost("~/api/proforma-invoice-receipts")]
        public async Task<JsonResult> SaveProformaInvoiceReceipt(SaveProformaInvoiceReceiptRequest saveProformaInvoiceReceiptRequest)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new SaveProformaInvoiceReceiptCommand(saveProformaInvoiceReceiptRequest.Id, saveProformaInvoiceReceiptRequest.ReceiptDate, saveProformaInvoiceReceiptRequest.Amount, saveProformaInvoiceReceiptRequest.PaymentMethod, saveProformaInvoiceReceiptRequest.ProformaInvoiceId, saveProformaInvoiceReceiptRequest.JobCardId, saveProformaInvoiceReceiptRequest.Remark,(int)userId));

            return new JsonResult(result);
        }

        [HttpDelete("~/api/proforma-invoice-receipts/{id:int}")]
        public async Task<JsonResult> RemoveProformaInvoiceReceipt(int id)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new RemoveProformaInvoiceReceiptCommand(id, (int)userId));

            return new JsonResult(result);
        }

        // ----- Receipts -----

        [HttpPost("~/api/receipts")]
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

        [HttpDelete("~/api/receipts/{id:int}")]
        public async Task<JsonResult> RemoveReceipt(int id)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new RemoveProformaInvoiceReceiptCommand(id, (int)userId));

            return new JsonResult(result);
        }

        [HttpPost("~/api/receipts/{id:int}/void")]
        public async Task<JsonResult> VoidReceipt(int id)
        {
            var userId = GetUserIdFromToken();
            var result = await mediator.Send(new VoidInvoiceReceiptCommand(id, (int)userId));

            return new JsonResult(result);
        }

        // ----- Reporting -----

        [HttpGet("~/api/reports/profit-and-loss")]
        public async Task<JsonResult> GetPnL(string? customerId, DateTime? dateFrom, DateTime? dateTo, string? jobCardNumber)
        {
            var userId = GetUserIdFromToken();
            var customerIds = customerId == null ? new List<int>() : customerId.Split(',').Select(Int32.Parse).ToList();
            var batches = await mediator.Send(new PnLEventQuery((int)userId, customerIds, dateFrom, dateTo,jobCardNumber));
            return new JsonResult(batches);
        }
    }
}
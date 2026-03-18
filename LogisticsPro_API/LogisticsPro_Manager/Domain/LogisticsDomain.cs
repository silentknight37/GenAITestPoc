using LogisticsPro_Common.DTO;
using LogisticsPro_Data.Repository;

namespace LogisticsPro_Manager.Domain
{
    public class LogisticsDomain : ILogisticsDomain
    {
        private LogisticsRepository logisticsRepository;
        public LogisticsDomain(LogisticsRepository logisticsRepository)
        {
            this.logisticsRepository = logisticsRepository;
        }

        public async Task<List<BatchLineItem>> GetBatchJobCardTransportation(DateTime? batchDate)
        {
            return await logisticsRepository.GetBatchJobCardTransportation(batchDate);
        }
        public async Task<BatchLineItemResponse> SaveBatchItem(BatchItemSaveRequest batchItemSaveRequest)
        {
            return await logisticsRepository.SaveBatchItem(batchItemSaveRequest);
        }
        public async Task<List<Batch>> GetBatches(bool isFirstLoad,string? batchCode, List<int>? vendorId, DateTime? batchDateFrom, DateTime? batchDateTo,string? jobCardNumber)
        {
            return await logisticsRepository.GetBatches(isFirstLoad,batchCode, vendorId, batchDateFrom, batchDateTo,jobCardNumber);
        }
        public async Task<bool> RemoveBatchItemFromList(int id)
        {
            return await logisticsRepository.RemoveBatchItemFromList(id);
        }

        public async Task<LineGenerateItem> GetBatchJobCardTransportationByVendorId(int vendorId, DateTime? fromDate, DateTime? toDate, string? jobCardNumber)
        {
            return await logisticsRepository.GetBatchJobCardTransportationByVendorId(vendorId,fromDate,toDate,jobCardNumber);
        }

        public async Task<int> SavePaymentVoucher(PaymentVoucherSaveRequest paymentVoucherSaveRequest)
        {
            return await logisticsRepository.SavePaymentVoucher(paymentVoucherSaveRequest);
        }

        public async Task<List<PaymentVoucher>> GetPaymentVouchers(bool isFirstLoad,string? paymentVoucherCode, string? invoiceNo, List<int>? vendorId, DateTime? paymentVoucherDateFrom, DateTime? paymentVoucherDateTo, string? jobCardNumber)
        {
            return await logisticsRepository.GetPaymentVouchers(isFirstLoad,paymentVoucherCode, invoiceNo, vendorId, paymentVoucherDateFrom, paymentVoucherDateTo,jobCardNumber);
        }
        public async Task<PaymentVoucher> GetPaymentVoucherById(int id)
        {
            return await logisticsRepository.GetPaymentVoucherById(id);
        }
        public async Task<bool> RemovePaymentVoucherItemFromList(int id,string itemType)
        {
            return await logisticsRepository.RemovePaymentVoucherItemFromList(id,itemType);
        }

        public async Task<LineGenerateItem> GetJobCardItemsByCustomerId(int customerId, DateTime? fromDate, DateTime? toDate, string? jobCardNumber)
        {
            return await logisticsRepository.GetJobCardItemsByCustomerId(customerId, fromDate, toDate, jobCardNumber);
        }
        public async Task<List<ProformaInvoice>> GetProformaInvoiceReceiptsByJobCardIds(List<int?> jobCardIds)
        {
            return await logisticsRepository.GetProformaInvoiceReceiptsByJobCardIds(jobCardIds);
        }

        public async Task<List<ProformaInvoice>> GetLinkedProformaInvoiceReceipts(int invoiceId)
        {
            return await logisticsRepository.GetLinkedProformaInvoiceReceipts(invoiceId);
        }

        public async Task<bool> SaveInvoice(InvoiceSaveRequest invoiceSaveRequest)
        {
            return await logisticsRepository.SaveInvoice(invoiceSaveRequest);
        }

        public async Task<List<Invoice>> GetInvoices(bool isFirstLoad, string? invoiceCode, List<int>? customerId, DateTime? invoiceDateFrom, DateTime? invoiceDateTo, DateTime? invoiceDueDateFrom, DateTime? invoiceDueDateTo, List<int>? statusId,string? jobCardNumber)
        {
            return await logisticsRepository.GetInvoices(isFirstLoad,invoiceCode, customerId, invoiceDateFrom, invoiceDateTo, invoiceDueDateFrom, invoiceDueDateTo,statusId, jobCardNumber);
        }
        public async Task<bool> VoidInvoice(int id, int userId)
        {
            return await logisticsRepository.VoidInvoice(id, userId);
        }

        public async Task<bool> SaveProformaInvoice(ProformaInvoiceSaveRequest proformaInvoiceSaveRequest)
        {
            return await logisticsRepository.SaveProformaInvoice(proformaInvoiceSaveRequest);
        }
        public async Task<bool> VoidProformaInvoice(int id,int userId)
        {
            return await logisticsRepository.VoidProformaInvoice(id, userId);
        }

        public async Task<List<ProformaInvoice>> GetProformaInvoices(bool isFirstLoad, string? invoiceCode, List<int>? customerId, DateTime? invoiceDateFrom, DateTime? invoiceDateTo, DateTime? invoiceDueDateFrom, DateTime? invoiceDueDateTo, List<int>? statusId, string? jobCardNumber)
        {
            return await logisticsRepository.GetProformaInvoices(isFirstLoad,invoiceCode, customerId, invoiceDateFrom, invoiceDateTo, invoiceDueDateFrom, invoiceDueDateTo,statusId,jobCardNumber);
        }

        public async Task<ProformaInvoice> GetProformaInvoice(int id)
        {
            return await logisticsRepository.GetProformaInvoice(id);
        }

        public async Task<List<ProformaInvoiceReceipt>> GetProformaInvoiceReceipts(int id)
        {
            return await logisticsRepository.GetProformaInvoiceReceipts(id);
        }
        public async Task<bool> SaveProformaInvoiceReceipt(ProformaInvoiceReceiptSaveRequest proformaInvoiceReceiptSaveRequest)
        {
            return await logisticsRepository.SaveProformaInvoiceReceipt(proformaInvoiceReceiptSaveRequest);
        }
        public async Task<bool> RemoveProformaInvoiceReceipt(int id)
        {
            return await logisticsRepository.RemoveProformaInvoiceReceipt(id);
        }
        //
        public async Task<Invoice> GetInvoice(int id)
        {
            return await logisticsRepository.GetInvoice(id);
        }

        public async Task<List<Receipt>> GetReceipts(int id)
        {
            return await logisticsRepository.GetReceipts(id);
        }
        public async Task<bool> SaveReceipt(ReceiptSaveRequest receiptSaveRequest)
        {
            return await logisticsRepository.SaveReceipt(receiptSaveRequest);
        }
        public async Task<bool> VoidInvoiceReceipt(int id)
        {
            return await logisticsRepository.VoidInvoiceReceipt(id);
        }
        //public async Task<bool> RemoveInvoiceReceipt(int id)
        //{
        //    return await logisticsRepository.RemoveProformaInvoiceReceipt(id);
        //}

        public async Task<List<PnL>> GetPnL(List<int>? customerId, DateTime? dateFrom, DateTime? dateTo, string? jobCardNumber)
        {
            return await logisticsRepository.GetPnL(customerId, dateFrom, dateTo,jobCardNumber);
        }
    }
}

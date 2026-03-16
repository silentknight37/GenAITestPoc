using LogisticsPro_Common.DTO;

namespace LogisticsPro_Data.Repository
{
    public interface ILogisticsRepository
    {
        Task<List<BatchLineItem>> GetBatchJobCardTransportation(DateTime? batchDate);
        Task<BatchLineItemResponse> SaveBatchItem(BatchItemSaveRequest batchItemSaveRequest);
        Task<List<Batch>> GetBatches(bool isFirstLoad, string? batchCode, List<int>? vendorId, DateTime? batchDateFrom, DateTime? batchDateTo, string? jobCardNumber);
        Task<bool> RemoveBatchItemFromList(int id);
        Task<LineGenerateItem> GetBatchJobCardTransportationByVendorId(int vendorId, DateTime? fromDate, DateTime? toDate, string? jobCardNumber);
        Task<int> SavePaymentVoucher(PaymentVoucherSaveRequest paymentVoucherSaveRequest);
        Task<List<PaymentVoucher>> GetPaymentVouchers(bool isFirstLoad, string? paymentVoucherCode, string? invoiceNo, List<int>? vendorId, DateTime? paymentVoucherDateFrom, DateTime? paymentVoucherDateTo, string? jobCardNumber);
        Task<PaymentVoucher> GetPaymentVoucherById(int id);
        Task<bool> RemovePaymentVoucherItemFromList(int id, string itemType);
        Task<LineGenerateItem> GetJobCardItemsByCustomerId(int customerId, DateTime? fromDate, DateTime? toDate,string? jobCardNumber);
        Task<bool> SaveInvoice(InvoiceSaveRequest invoiceSaveRequest);
        Task<List<Invoice>> GetInvoices(bool isFirstLoad,string? invoiceCode, List<int>? customerId, DateTime? invoiceDateFrom, DateTime? invoiceDateTo, DateTime? invoiceDueDateFrom, DateTime? invoiceDueDateTo, List<int>? statusId, string? jobCardNumber);
        Task<bool> VoidInvoice(int id, int userId);
        Task<bool> SaveProformaInvoice(ProformaInvoiceSaveRequest proformaInvoiceSaveRequest);
        Task<bool> VoidProformaInvoice(int id, int userId);
        Task<List<ProformaInvoice>> GetProformaInvoices(bool isFirstLoad, string? invoiceCode, List<int>? customerId, DateTime? invoiceDateFrom, DateTime? invoiceDateTo, DateTime? invoiceDueDateFrom, DateTime? invoiceDueDateTo, List<int>? statusId, string? jobCardNumber);
        Task<ProformaInvoice> GetProformaInvoice(int id);
        Task<List<ProformaInvoiceReceipt>> GetProformaInvoiceReceipts(int id);
        Task<bool> RemoveProformaInvoiceReceipt(int id);
        Task<bool> SaveProformaInvoiceReceipt(ProformaInvoiceReceiptSaveRequest proformaInvoiceReceiptSaveRequest);
        Task<List<ProformaInvoice>> GetProformaInvoiceReceiptsByJobCardIds(List<int?> jobCardIds);
        Task<List<ProformaInvoice>> GetLinkedProformaInvoiceReceipts(int invoiceId);
        Task<Invoice> GetInvoice(int id);
        Task<List<Receipt>> GetReceipts(int id);
        Task<bool> SaveReceipt(ReceiptSaveRequest receiptSaveRequest);
        Task<List<PnL>> GetPnL(List<int>? customerId, DateTime? dateFrom, DateTime? dateTo, string? jobCardNumber);
        Task<bool> VoidInvoiceReceipt(int id);
    }
}

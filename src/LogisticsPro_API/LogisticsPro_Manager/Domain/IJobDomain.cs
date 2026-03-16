using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Domain
{
    public interface IJobDomain
    {
        Task<List<JobCard>> GetJobs(bool isFirstLoad,string? jobCardCode, string? jobCardDescription, List<int>? customerId, DateTime? effectiveDateFrom, DateTime? effectiveDateTo, List<int>? statusIds);
        Task<int> SaveJob(JobSaveRequest jobSaveRequest);
        Task<int> CloseJobCard(CloseJobCardRequest closeJobCardRequest);
        Task<int> OpenJobCard(OpenJobCardRequest openJobCardRequest);
        Task<JobCard> GetJob(int jobCardId);
        Task<List<JobCardTransportation>> GetJobTransportations(int jobCardId);
        Task<List<JobCardTransportation>> GetCostTransportation(string? jobCardCode, string? bookingRef, string? batchNo, string? clientRef, int? customerId, int? vendorId, DateTime? dateFrom, DateTime? dateTo);
        Task<List<JobCardHotel>> GetCostHotel(string? jobCardCode, string? bookingRef, string? batchNo, string? clientRef, int? customerId, int? vendorId, DateTime? dateFrom, DateTime? dateTo);
        Task<List<JobCardVisa>> GetCostVisa(string? jobCardCode, string? bookingRef, string? batchNo, string? clientRef, int? customerId, int? vendorId, DateTime? dateFrom, DateTime? dateTo);
        Task<List<JobCardMiscellaneous>> GetCostMiscellaneous(string? jobCardCode, string? bookingRef, string? batchNo, string? clientRef, int? customerId, int? vendorId, DateTime? dateFrom, DateTime? dateTo);
        Task<JobCardTransportation> GetJobTransportation(int id);
        Task<List<JobCardHotel>> GetJobHotels(int jobCardId);
        Task<JobCardHotel> GetJobHotel(int id);
        Task<List<JobCardVisa>> GetJobVisas(int jobCardId);
        Task<JobCardVisa> GetJobVisa(int id);
        Task<List<JobCardMiscellaneous>> GetJobMiscellanea(int jobCardId, int userId);
        Task<JobCardMiscellaneous> GetJobMiscellaneous(int id,int userId);
        Task<JobCardFinance> GetJobFinancRreceiptAndPayments(int jobCardId);
        Task<bool> SaveJobTransportation(JobCardTransportationRequest jobCardTransportationRequest);
        Task<bool> RemoveTransportation(int id);
        Task<bool> SaveJobHotel(JobCardHotelRequest jobCardHotelRequest);
        Task<bool> RemoveHotel(int id);
        Task<bool> SaveJobVisa(JobCardVisaRequest jobCardVisaRequest);
        Task<bool> RemoveVisa(int id);
        Task<bool> SaveJobMiscellaneous(JobCardMiscellaneousRequest jobCardVisaRequest);
        Task<bool> RemoveMiscellaneous(int id);
        Task<LineGenerateItem> GetFullReportItems(string? jobCardCode, string? bookingRef, string? batchNo, string? clientRef, List<int>? customerId, List<int>? vendorId, DateTime? dateFrom, DateTime? dateTo);
        Task<LineGenerateItem> GetUnInvoiceReportItems(string? jobCardCode, string? bookingRef, string? batchNo, string? clientRef, List<int>? customerId, List<int>? vendorId, DateTime? dateFrom, DateTime? dateTo);
        Task<List<ReportItem>> GetDailyReportItems(string? bookingRef, string? batchNo, string? clientRef, List<int>? customerId, List<int>? vendorId, DateTime? dateFrom);
        Task<bool> UpdateCost(UpdateCostRequest updateCostRequest);
        Task<bool> RemoveJobCard(int id);
        Task<HistoryGenerateItem> GetHistoryReportItems(string? jobCardCode, List<int>? userId, DateTime? dateFrom, DateTime? dateTo);
    }
}

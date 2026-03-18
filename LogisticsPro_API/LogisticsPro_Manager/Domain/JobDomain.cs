using LogisticsPro_Common.DTO;
using LogisticsPro_Data.Repository;

namespace LogisticsPro_Manager.Domain
{
    public class JobDomain : IJobDomain
    {
        private JobRepository jobRepository;
        public JobDomain(JobRepository jobRepository)
        {
            this.jobRepository = jobRepository;
        }

        public async Task<List<JobCard>> GetJobs(bool isFirstLoad, string? jobCardCode, string? jobCardDescription, List<int>? customerId, DateTime? effectiveDateFrom, DateTime? effectiveDateTo, List<int>? statusIds)
        {
            return await jobRepository.GetJobs(isFirstLoad,jobCardCode, jobCardDescription, customerId, effectiveDateFrom, effectiveDateTo,statusIds);
        }
        public async Task<List<JobCardTransportation>> GetCostTransportation(string? jobCardCode, string? bookingRef, string? batchNo, string? clientRef, int? customerId, int? vendorId, DateTime? dateFrom, DateTime? dateTo)
        {
            return await jobRepository.GetCostTransportation(jobCardCode, bookingRef, batchNo, clientRef, customerId, vendorId, dateFrom, dateTo);
        }
        public async Task<List<JobCardHotel>> GetCostHotel(string? jobCardCode, string? bookingRef, string? batchNo, string? clientRef, int? customerId, int? vendorId, DateTime? dateFrom, DateTime? dateTo)
        {
            return await jobRepository.GetCostHotel(jobCardCode, bookingRef, batchNo, clientRef, customerId, vendorId, dateFrom, dateTo);
        }
        public async Task<List<JobCardVisa>> GetCostVisa(string? jobCardCode, string? bookingRef, string? batchNo, string? clientRef, int? customerId, int? vendorId, DateTime? dateFrom, DateTime? dateTo)
        {
            return await jobRepository.GetCostVisa(jobCardCode, bookingRef, batchNo, clientRef, customerId, vendorId, dateFrom, dateTo);
        }
        public async Task<List<JobCardMiscellaneous>> GetCostMiscellaneous(string? jobCardCode, string? bookingRef, string? batchNo, string? clientRef, int? customerId, int? vendorId, DateTime? dateFrom, DateTime? dateTo)
        {
            return await jobRepository.GetCostMiscellaneous(jobCardCode, bookingRef, batchNo, clientRef, customerId, vendorId, dateFrom, dateTo);
        }
        public async Task<int> SaveJob(JobSaveRequest jobSaveRequest)
        {
            return await jobRepository.SaveJob(jobSaveRequest);
        }
        public async Task<bool> RemoveJobCard(int id)
        {
            return await jobRepository.RemoveJobCard(id);
        }
        public async Task<int> CloseJobCard(CloseJobCardRequest closeJobCardRequest)
        {
            return await jobRepository.CloseJobCard(closeJobCardRequest);
        }
        public async Task<int> OpenJobCard(OpenJobCardRequest openJobCardRequest)
        {
            return await jobRepository.OpenJobCard(openJobCardRequest);
        }
        public async Task<JobCard> GetJob(int jobCardId)
        {
            return await jobRepository.GetJob(jobCardId);
        }
        public async Task<List<JobCardTransportation>> GetJobTransportations(int jobCardId)
        {
            return await jobRepository.GetJobTransportations(jobCardId);
        }
        public async Task<JobCardTransportation> GetJobTransportation(int id)
        {
            return await jobRepository.GetJobTransportation(id);
        }
        public async Task<List<JobCardHotel>> GetJobHotels(int jobCardId)
        {
            return await jobRepository.GetJobHotels(jobCardId);
        }
        public async Task<JobCardHotel> GetJobHotel(int id)
        {
            return await jobRepository.GetJobHotel(id);
        }
        public async Task<List<JobCardVisa>> GetJobVisas(int jobCardId)
        {
            return await jobRepository.GetJobVisas(jobCardId);
        }
        public async Task<JobCardVisa> GetJobVisa(int id)
        {
            return await jobRepository.GetJobVisa(id);
        }
        public async Task<List<JobCardMiscellaneous>> GetJobMiscellanea(int jobCardId,int userId)
        {
            return await jobRepository.GetJobMiscellanea(jobCardId,userId);
        }
        public async Task<JobCardMiscellaneous> GetJobMiscellaneous(int id,int userId)
        {
            return await jobRepository.GetJobMiscellaneous(id,userId);
        }
        public async Task<JobCardFinance> GetJobFinancRreceiptAndPayments(int jobCardId)
        {
            return await jobRepository.GetJobFinancRreceiptAndPayments(jobCardId);
        }
        public async Task<bool> SaveJobTransportation(JobCardTransportationRequest jobCardTransportationRequest)
        {
            return await jobRepository.SaveJobTransportation(jobCardTransportationRequest);
        }
        public async Task<bool> RemoveTransportation(int id)
        {
            return await jobRepository.RemoveTransportation(id);
        }
        public async Task<bool> SaveJobHotel(JobCardHotelRequest jobCardHotelRequest)
        {
            return await jobRepository.SaveJobHotel(jobCardHotelRequest);
        }
        public async Task<bool> RemoveHotel(int id)
        {
            return await jobRepository.RemoveHotel(id);
        }
        public async Task<bool> SaveJobVisa(JobCardVisaRequest jobCardVisaRequest)
        {
            return await jobRepository.SaveJobVisa(jobCardVisaRequest);
        }
        public async Task<bool> RemoveVisa(int id)
        {
            return await jobRepository.RemoveVisa(id);
        }
        public async Task<bool> SaveJobMiscellaneous(JobCardMiscellaneousRequest jobCardMiscellaneousRequest)
        {
            return await jobRepository.SaveJobMiscellaneous(jobCardMiscellaneousRequest);
        }
        public async Task<bool> RemoveMiscellaneous(int id)
        {
            return await jobRepository.RemoveMiscellaneous(id);
        }
        public async Task<LineGenerateItem> GetFullReportItems(string? jobCardCode, string? bookingRef, string? batchNo, string? clientRef, List<int>? customerId, List<int>? vendorId, DateTime? dateFrom, DateTime? dateTo)
        {
            return await jobRepository.GetFullReportItems(jobCardCode, bookingRef, batchNo, clientRef, customerId, vendorId, dateFrom, dateTo);
        }
        public async Task<HistoryGenerateItem> GetHistoryReportItems(string? jobCardCode, List<int>? userId, DateTime? dateFrom, DateTime? dateTo)
        {
            return await jobRepository.GetHistoryReportItems(jobCardCode, userId, dateFrom, dateTo);
        }
        public async Task<LineGenerateItem> GetUnInvoiceReportItems(string? jobCardCode, string? bookingRef, string? batchNo, string? clientRef, List<int>? customerId, List<int>? vendorId, DateTime? dateFrom, DateTime? dateTo)
        {
            return await jobRepository.GetUnInvoiceReportItems(jobCardCode, bookingRef, batchNo, clientRef, customerId, vendorId, dateFrom, dateTo);
        }
        public async Task<List<ReportItem>> GetDailyReportItems(string? bookingRef, string? batchNo, string? clientRef, List<int>? customerId, List<int>? vendorId, DateTime? dateFrom)
        {
            return await jobRepository.GetDailyReportItems(bookingRef, batchNo, clientRef, customerId, vendorId, dateFrom);
        }
        public async Task<bool> UpdateCost(UpdateCostRequest updateCostRequest)
        {
            return await jobRepository.UpdateCost(updateCostRequest);
        }
    }
}

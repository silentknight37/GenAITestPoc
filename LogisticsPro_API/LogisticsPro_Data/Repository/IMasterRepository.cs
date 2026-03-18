using LogisticsPro_Common.DTO;

namespace LogisticsPro_Data.Repository
{
    public interface IMasterRepository
    {
        Task<List<Customer>> GetCustomers();
        Task<bool> SaveCustomer(CustomerSaveRequest customerSaveRequest);
        Task<bool> RemoveCustomer(long id);
        Task<List<Vendor>> GetVendors();
        Task<bool> SaveVendor(VendorSaveRequest vendorSaveRequest);
        Task<bool> RemoveVendor(long id);
        Task<List<JobCard>> GetJobCards(int customerId);
        Task<List<SystemRole>> GetSystemRoles();
        Task<bool> SaveUser(UserSaveRequest userSaveRequest);
        Task<List<User>> GetUsers();
        Task<bool> IsUserAlreadyExsitis(string userName);
        Task<User?> GetUserByLoginDetails(string userName, string password);
        Task<bool> UpdateLastLogin(int userId);
        Task<List<PaymentMethod>> GetPaymentMethods();
        Task<List<InvoiceStatus>> GetInvoiceStatus();
        Task<bool> SaveEvent(EventSaveRequest eventSaveRequest);
        Task<List<Event>> GetEvents();
        Task<bool> RemoveEvent(long id);
        Task<Dashboard> GetDashboard(int userId);
        Task<List<JobCardTransportation>> GetTransports();
        Task<List<JobCardHotel>> GetHotels();
        Task<List<Country>> GetCountries();
    }
}

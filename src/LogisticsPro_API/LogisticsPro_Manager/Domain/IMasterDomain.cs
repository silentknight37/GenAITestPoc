using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Domain
{
    public interface IMasterDomain
    {
        Task<List<Customer>> GetCustomers();
        Task<bool> SaveCustomer(CustomerSaveRequest customerSaveRequest);
        Task<bool> RemoveCustomer(int customerId);
        Task<List<Vendor>> GetVendors();
        Task<bool> SaveVendor(VendorSaveRequest vendorSaveRequest);
        Task<bool> RemoveVendor(int vendorId);
        Task<List<Country>> GetCountries();
        Task<List<JobCard>> GetJobCards(int customerId);
        Task<List<SystemRole>> GetSystemRoles();
        Task<List<User>> GetUsers();
        Task<bool> SaveUser(UserSaveRequest userSaveRequest);
        Task<bool> IsUserAlreadyExsitis(string userName);
        Task<User?> GetUserByLoginDetails(string userName, string password);
        Task<bool> UpdateLastLogin(int userId);
        Task<List<PaymentMethod>> GetPaymentMethods();
        Task<List<InvoiceStatus>> GetInvoiceStatus();
        Task<List<Event>> GetEvents();
        Task<bool> SaveEvent(EventSaveRequest eventSaveRequest);
        Task<bool> RemoveEvent(int eventId);
        Task<Dashboard> GetDashboard(int userId);
        Task<List<JobCardTransportation>> GetTransports();
        Task<List<JobCardHotel>> GetHotels();
    }
}

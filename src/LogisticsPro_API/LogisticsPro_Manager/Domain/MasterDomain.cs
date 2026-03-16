using LogisticsPro_Common.DTO;
using LogisticsPro_Data.Repository;
using Newtonsoft.Json;
using System;
using System.Net.Http.Headers;

namespace LogisticsPro_Manager.Domain
{
    public class MasterDomain: IMasterDomain
    {
        private MasterRepository masterRepository;
        public MasterDomain(MasterRepository masterRepository)
        {
            this.masterRepository = masterRepository;
        }

        public async Task<List<Customer>> GetCustomers()
        {
            return await masterRepository.GetCustomers();
        }

        public async Task<bool> SaveCustomer(CustomerSaveRequest customerSaveRequest)
        {
            return await masterRepository.SaveCustomer(customerSaveRequest);
        }

        public async Task<bool> RemoveCustomer(int customerId)
        {
            return await masterRepository.RemoveCustomer(customerId);
        }

        public async Task<List<Vendor>> GetVendors()
        {
            return await masterRepository.GetVendors();
        }

        public async Task<List<Country>> GetCountries()
        {
            //using var client = new HttpClient();
            //string apiUri = $"https://restcountries.com/v3.1/all";
            //HttpResponseMessage response = client.GetAsync(apiUri).Result;

            //var countries = JsonConvert.DeserializeObject<List<Country>>(response.Content.ReadAsStringAsync().Result);

            //return countries;
            return await masterRepository.GetCountries();
        }

        public async Task<bool> SaveVendor(VendorSaveRequest vendorSaveRequest)
        {
            return await masterRepository.SaveVendor(vendorSaveRequest);
        }

        public async Task<bool> RemoveVendor(int vendorId)
        {
            return await masterRepository.RemoveVendor(vendorId);
        }

        public async Task<List<JobCard>> GetJobCards(int customerId)
        {
            return await masterRepository.GetJobCards(customerId);
        }

        public async Task<List<SystemRole>> GetSystemRoles()
        {
            return await masterRepository.GetSystemRoles();
        }
        public async Task<List<PaymentMethod>> GetPaymentMethods()
        {
            return await masterRepository.GetPaymentMethods();
        }
        public async Task<List<InvoiceStatus>> GetInvoiceStatus()
        {
            return await masterRepository.GetInvoiceStatus();
        }
        public async Task<List<User>> GetUsers()
        {
            return await masterRepository.GetUsers();
        }

        public async Task<List<JobCardTransportation>> GetTransports()
        {
            return await masterRepository.GetTransports();
        }
        public async Task<List<JobCardHotel>> GetHotels()
        {
            return await masterRepository.GetHotels();
        }


        public async Task<bool> SaveUser(UserSaveRequest userSaveRequest)
        {
            return await masterRepository.SaveUser(userSaveRequest);
        }

        public async Task<bool> IsUserAlreadyExsitis(string userName)
        {
            return await masterRepository.IsUserAlreadyExsitis(userName);
        }

        public async Task<User?> GetUserByLoginDetails(string userName,string password)
        {
            return await masterRepository.GetUserByLoginDetails(userName,password);
        }
        public async Task<bool> UpdateLastLogin(int userId)
        {
            return await masterRepository.UpdateLastLogin(userId);
        }

        public async Task<List<Event>> GetEvents()
        {
            return await masterRepository.GetEvents();
        }

        public async Task<bool> SaveEvent(EventSaveRequest eventSaveRequest)
        {
            return await masterRepository.SaveEvent(eventSaveRequest);
        }

        public async Task<bool> RemoveEvent(int eventId)
        {
            return await masterRepository.RemoveEvent(eventId);
        }

        public async Task<Dashboard> GetDashboard(int userId)
        {
            return await masterRepository.GetDashboard(userId);
        }
    }
}

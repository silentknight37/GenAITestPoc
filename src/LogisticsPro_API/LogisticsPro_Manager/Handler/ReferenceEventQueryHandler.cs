using LogisticsPro_Common.DTO;
using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class ReferenceEventQueryHandler : IRequestHandler<ReferenceEventQuery, ReferanceQueryEnvelop>
    {
        private readonly IMasterFactory masterFactory;

        public ReferenceEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.masterFactory = new MasterFactory(dB_LogisticsproContext);
        }

        public async Task<ReferanceQueryEnvelop> Handle(ReferenceEventQuery referenceEventQuery, CancellationToken cancellationToken)
        {
            var masterDomain = this.masterFactory.Create();
            var customers = await masterDomain.GetCustomers();
            var vendors = await masterDomain.GetVendors();
            var countries = await masterDomain.GetCountries();
            var systemRoles = await masterDomain.GetSystemRoles();
            var paymentMethods = await masterDomain.GetPaymentMethods();
            var invoiceStatuses = await masterDomain.GetInvoiceStatus();
            var users = await masterDomain.GetUsers();
            var transfers = await masterDomain.GetTransports();
            var hotels = await masterDomain.GetHotels();

            var customerRefData = customers.Select(i => new ReferenceData { value = i.Id, code = i.CustomerName }).ToList();
            var vendorRefData = vendors.Select(i => new ReferenceVendorData { value = i.Id, code = i.VendorName,vendorType=i.SelectedVendorTypes }).ToList();
            var countryRefData = countries.Select(i => new ReferenceData { value = i.Ccn3, code = i.Name }).ToList();
            var roleData = systemRoles.Select(i => new ReferenceData { value = i.Id, code = i.RoleName }).ToList();
            var paymentMethodData = paymentMethods.Select(i => new ReferenceData { value = i.Id, code = i.PaymentMethodName }).ToList();
            var invoiceStatuseData = invoiceStatuses.Select(i => new ReferenceData { value = i.Id, code = i.StatusName }).ToList();
            var userData = users.Select(i => new ReferenceData { value = i.Id, code = i.UserName }).ToList();

            var pickUpLocationData = transfers.Where(i=>!string.IsNullOrEmpty(i.PickupLocation)).GroupBy(i=>i.PickupLocation.Trim()).Select(i => new AutoTypeReferenceData { value = i.Key.Trim(), code = i.Key.Trim() }).ToList();
            var dropOffLocationData = transfers.Where(i => !string.IsNullOrEmpty(i.DropoffLocation)).GroupBy(i => i.DropoffLocation.Trim()).Select(i => new AutoTypeReferenceData { value = i.Key.Trim(), code = i.Key.Trim() }).ToList();
            var vehicleTypeData = transfers.Where(i => !string.IsNullOrEmpty(i.VehicleType)).GroupBy(i => i.VehicleType.Trim()).Select(i => new AutoTypeReferenceData { value = i.Key.Trim(), code = i.Key.Trim() }).ToList();
            var hotelNameData = hotels.Where(i => !string.IsNullOrEmpty(i.HotelName)).GroupBy(i => i.HotelName.Trim()).Select(i => new AutoTypeReferenceData { value = i.Key.Trim(), code = i.Key.Trim() }).ToList();

            var statusData = new List<ReferenceData> { new ReferenceData { value = 1, code = "Active" }, new ReferenceData { value = 1, code = "Deactivated" } };
            var jobCardStatusData = new List<ReferenceData> { new ReferenceData { value = 1, code = "Open" }, new ReferenceData { value = 2, code = "Close" } };
            var referenceType = new List<ReferenceData> { new ReferenceData { code = "Transportation", value = 1 }, new ReferenceData { code = "Hotel", value = 2 }, new ReferenceData { code = "Visa", value = 3 }, new ReferenceData { code = "Miscellaneous", value = 4 } };

            return new ReferanceQueryEnvelop(customerRefData, vendorRefData, referenceType, countryRefData, roleData, statusData, paymentMethodData, invoiceStatuseData, userData, pickUpLocationData,dropOffLocationData,vehicleTypeData,hotelNameData, jobCardStatusData);
        }
    }
}

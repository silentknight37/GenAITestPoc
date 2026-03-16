using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Envelope
{
    public class ReferanceQueryEnvelop
    {
        public ReferanceQueryEnvelop(List<ReferenceData> customers, List<ReferenceVendorData> vendors, List<ReferenceData> referenceType,List<ReferenceData> countries, List<ReferenceData> roles, List<ReferenceData> status, List<ReferenceData> paymentMethods, List<ReferenceData> invoiceStatuse, List<ReferenceData> users,
            List<AutoTypeReferenceData> pickUpLocations, List<AutoTypeReferenceData> dropOffLocations, List<AutoTypeReferenceData> vehicleTypes, List<AutoTypeReferenceData> hotelNames, List<ReferenceData> jobCardStatus)
        {
            this.Customers = customers;
            this.Vendors = vendors;
            this.ReferenceType = referenceType;
            this.Countries = countries;
            this.Status = status;
            this.Roles = roles;
            this.PaymentMethods = paymentMethods;
            this.InvoiceStatuse = invoiceStatuse;
            this.Users = users;
            this.PickUpLocations = pickUpLocations;
            this.DropOffLocations = dropOffLocations;
            this.VehicleTypes = vehicleTypes;
            this.HotelNames = hotelNames;
            this.JobCardStatus = jobCardStatus;
        }

        public List<ReferenceData> Customers { get; set; }
        public List<ReferenceVendorData> Vendors { get; set; }
        public List<ReferenceData> ReferenceType { get; set; }
        public List<ReferenceData> Countries { get; set; }
        public List<ReferenceData> Roles { get; set; }
        public List<ReferenceData> Status { get; set; }
        public List<ReferenceData> PaymentMethods { get; set; }
        public List<ReferenceData> InvoiceStatuse { get; set; }
        public List<ReferenceData> JobCardStatus { get; set; }
        public List<ReferenceData> Users { get; set; }
        public List<AutoTypeReferenceData> PickUpLocations { get; set; }
        public List<AutoTypeReferenceData> DropOffLocations { get; set; }
        public List<AutoTypeReferenceData> VehicleTypes { get; set; }
        public List<AutoTypeReferenceData> HotelNames { get; set; }

    }
}

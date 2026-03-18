using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Envelope
{
    public class VendorQueryEnvelop
    {
        public VendorQueryEnvelop(List<Vendor> vendors)
        {
            this.Vendors = vendors;
        }

        public List<Vendor> Vendors { get; set; }
    }
}

using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Envelope
{
    public class JobCardHotelQueryEnvelop
    {
        public JobCardHotelQueryEnvelop(JobCardHotel jobCardHotel)
        {
            this.JobCardHotel = jobCardHotel;
        }

        public JobCardHotel JobCardHotel { get; set; }
    }
}

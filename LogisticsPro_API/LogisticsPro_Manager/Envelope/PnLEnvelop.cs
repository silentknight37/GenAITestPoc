using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Envelope
{
    public class PnLEnvelop
    {
        public PnLEnvelop(List<PnL> pnL)
        {
            this.PnL = pnL;
        }

        public List<PnL> PnL { get; set; }
    }
}

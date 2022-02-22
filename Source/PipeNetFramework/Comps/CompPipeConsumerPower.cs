using RimWorld;

namespace PipeNetFramework.Comps
{
    public class CompPipeConsumerPower : CompPipeConsumer
    {
        private CompPowerPlant powerPlant;
        public CompPowerPlant PowerPlant => powerPlant ??= parent.GetComp<CompPowerPlant>();

        public override bool TryConsume()
        {
            if (PowerPlant == null) return false;

            if (PowerPlant.breakdownableComp?.BrokenDown == true ||
                PowerPlant.refuelableComp?.HasFuel == true ||
                PowerPlant.flickableComp?.SwitchIsOn == false ||
                PowerPlant.autoPoweredComp?.WantsToBeOn == false ||
                !base.TryConsume())
            {
                PowerPlant.PowerOn = false;
                return false;
            }

            PowerPlant.PowerOn = true;
            return true;

        }
    }
}

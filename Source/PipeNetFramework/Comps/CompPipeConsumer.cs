using RimWorld;
using Verse;

namespace PipeNetFramework.Comps
{
    [HotSwappable]
    public class CompPipeConsumer : CompPipe
    {
        protected CompPowerTrader Power { get; private set; }
        protected CompFlickable Flickable { get; private set; }
        protected CompBreakdownable Breakdownable { get; private set; }
        protected CompRefuelable Refuelable { get; private set; }

        public bool ConsumedThisTick { get; private set; } = false;

        public new CompProperties_PipeConsumer Props => (CompProperties_PipeConsumer)props;

        public override void CompTick() => TryConsume();
        public override void CompTickRare() => TryConsume();
        public override void CompTickLong() => TryConsume();

        public virtual bool TryConsume()
        {
            if (Network.StoredVolume.TryGetValue(Props.consumedThing) < Props.consumerCount)
            {
                ConsumedThisTick = false;
                return false;
            }

            if (Power?.PowerOn == false)
            {
                ConsumedThisTick = false;
                return false;
            }

            if (Flickable?.SwitchIsOn == false)
            {
                ConsumedThisTick = false;
                return false;
            }

            if (Breakdownable?.BrokenDown == true)
            {
                ConsumedThisTick = false;
                return false;
            }

            if (Refuelable?.HasFuel == false)
            {
                ConsumedThisTick = false;
                return false;
            }

            return ConsumedThisTick = Network.TryDrain(Props.consumerCount, Props.consumedThing);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            Power = parent.GetComp<CompPowerTrader>();
            Flickable = parent.GetComp<CompFlickable>();
            Breakdownable = parent.GetComp<CompBreakdownable>();
            Refuelable = parent.GetComp<CompRefuelable>();
        }
    }
}

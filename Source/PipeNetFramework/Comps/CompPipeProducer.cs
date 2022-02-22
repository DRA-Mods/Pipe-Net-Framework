using RimWorld;
using Verse;

namespace PipeNetFramework.Comps
{
    [HotSwappable]
    public class CompPipeProducer : CompPipe
    {
        protected CompPowerTrader Power { get; private set; }
        protected CompFlickable Flickable { get; private set; }
        protected CompBreakdownable Breakdownable { get; private set; }
        protected CompRefuelable Refuelable { get; private set; }
        protected CompPipeConsumer PipeConsumer { get; private set; }

        public new CompProperties_PipeProducer Props => (CompProperties_PipeProducer)props;

        public override void CompTick() => Produce();
        public override void CompTickRare() => Produce();
        public override void CompTickLong() => Produce();

        protected virtual void Produce()
        {
            if (Network.EmptyVolume.TryGetValue(Props.producedThing) > 0 && 
                Power?.PowerOn != false && 
                Flickable?.SwitchIsOn != false && 
                Breakdownable?.BrokenDown != true && 
                Refuelable?.HasFuel != false && 
                PipeConsumer?.ConsumedThisTick != false)
                Network.Fill(Props.producedCount, Props.producedThing);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            Power = parent.GetComp<CompPowerTrader>();
            Flickable = parent.GetComp<CompFlickable>();
            Breakdownable = parent.GetComp<CompBreakdownable>();
            Refuelable = parent.GetComp<CompRefuelable>();
            PipeConsumer = parent.GetComp<CompPipeConsumer>();
        }
    }
}

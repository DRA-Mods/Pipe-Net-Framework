using RimWorld;

namespace PipeNetFramework.Comps
{
    public class CompPowerPlant_PipeConsumer : CompPowerPlant
    {
        protected CompPipeConsumer ConsumerComp { get; private set; }

        private OverlayHandle? overlayNeedsResource;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            ConsumerComp = parent.GetComp<CompPipeConsumer>();
        }

        public override void CompTick()
        {
            base.CompTick();

            switch (PowerOutput, ConsumerComp?.ConsumedThisTick)
            {
                case ( > 0f, false):
                    UpdateOverlays(false);
                    PowerOutput = 0f;
                    break;
                case (0f, true):
                    UpdateOverlays(true);
                    PowerOutput = DesiredPowerOutput;
                    break;
                default:
                    PowerOutput = PowerOutput;
                    break;
            }
        }

        private void UpdateOverlays(bool producesPower)
        {
            parent.Map.overlayDrawer.Disable(parent, ref overlayNeedsResource);
            if (!producesPower)
                overlayNeedsPower = parent.Map.overlayDrawer.Enable(parent, OverlayTypes.OutOfFuel);
        }
    }
}

using PipeNetFramework.Comps;
using PipeNetFramework.MapComps;
using PipeNetFramework.Utilities;
using UnityEngine;
using Verse;

namespace PipeNetFramework.Overlay
{
    [HotSwappable]
    public class SectionLayer_PipeNetwork : SectionLayer_Things
    {
        private static int lastGasGridDrawFrame;
        public virtual bool ShouldDrawPipeNet => lastGasGridDrawFrame + 1 >= Time.frameCount;
        public static void DrawGasPipeOverlayThisFrame() => lastGasGridDrawFrame = Time.frameCount;

        public SectionLayer_PipeNetwork(Section section) : base(section)
        {
            requireAddToMapMesh = false;
            relevantChangeTypes = MapMeshFlag.Buildings | (MapMeshFlag)1024;
        }

        public override void DrawLayer()
        {
            if (ShouldDrawPipeNet)
                base.DrawLayer();
        }

        public override void TakePrintFrom(Thing thing)
        {
            if (thing.Faction?.IsPlayer != true) return;

            var comp = thing.TryGetComp<CompPipe>();
            if (comp == null)
                return;

            if (!ModGraphics.PipeOverlays.TryGetValue(comp.GetId(), out var graphic))
                graphic = ModGraphics.PipeOverlays[PipeNetMapComp.NullNetName];
            graphic.Print(this, thing, 0f);
        }
    }
}

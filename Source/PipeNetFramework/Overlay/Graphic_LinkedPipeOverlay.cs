using PipeNetFramework.Comps;
using PipeNetFramework.MapComps;
using RimWorld;
using Verse;

namespace PipeNetFramework.Overlay
{
    [HotSwappable]
    public class Graphic_LinkedPipeOverlay : Graphic_LinkedTransmitterOverlay
    {
        public Graphic_LinkedPipeOverlay(Graphic graphic) : base(graphic)
        { }

        public override bool ShouldLinkWith(IntVec3 c, Thing parent)
        {
            var comp = parent.TryGetComp<CompPipe>();
            if (comp == null) return false;

            return c.InBounds(parent.Map) && parent.Map.GetComponent<PipeNetMapComp>().PipeNetAt(c, comp.Props.netTypeDef) != null;
        }
    }
}

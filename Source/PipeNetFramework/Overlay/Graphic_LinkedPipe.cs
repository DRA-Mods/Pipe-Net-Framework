using PipeNetFramework.Comps;
using PipeNetFramework.MapComps;
using UnityEngine;
using Verse;

namespace PipeNetFramework.Overlay
{
    [HotSwappable]
    public class Graphic_LinkedPipe : Graphic_Linked
    {
        public Graphic_LinkedPipe(Graphic graphic) : base(graphic)
        { }

        public override bool ShouldLinkWith(IntVec3 c, Thing parent)
        {
            var comp = parent.TryGetComp<CompPipe>();
            if (comp == null) return false;

            return c.InBounds(parent.Map) && parent.Map.GetComponent<PipeNetMapComp>().PipeNetAt(c, comp.Props.netTypeDef) != null;
        }

        public override void Print(SectionLayer layer, Thing thing, float extraRotation)
        {
            base.Print(layer, thing, extraRotation);

            foreach (var cell in GenAdj.CellsAdjacentCardinal(thing))
            {
                if (!cell.InBounds(thing.Map)) continue;
                if (!cell.GetThingList(thing.Map).Any(x => x.TryGetComp<CompPipe>() != null && x.def.graphicData.Linked)) continue;

                var mat = LinkedDrawMatFrom(thing, cell);
                Printer_Plane.PrintPlane(layer, cell.ToVector3ShiftedWithAltitude(thing.def.Altitude), Vector2.one, mat);
            }
        }
    }
}

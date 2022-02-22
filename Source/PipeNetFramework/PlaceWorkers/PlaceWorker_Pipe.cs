using PipeNetFramework.Comps;
using Verse;

namespace PipeNetFramework.PlaceWorkers
{
    [HotSwappable]
    public class PlaceWorker_Pipe : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            var thingList = loc.GetThingList(map);
            foreach (var t in thingList)
            {
                switch (t.def.entityDefToBuild ?? t.def)
                {
                    case ThingDef thingDef when thingDef.CompDefForAssignableFrom<CompPipe>() is CompProperties_Pipe {isPipe: false}:
                        return "PNF_CannotReplaceNonPipe".Translate();
                }
            }
            return true;
        }
    }
}
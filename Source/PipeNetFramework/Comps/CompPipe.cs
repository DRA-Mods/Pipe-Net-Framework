using System.Linq;
using System.Text;
using PipeNetFramework.MapComps;
using PipeNetFramework.Overlay;
using PipeNetFramework.PipeNet;
using Verse;

namespace PipeNetFramework.Comps
{
    [HotSwappable]
    public class CompPipe : ThingComp
    {
        // Backing fields
        private PipeNetMapComp mapComp;

        public PipeNetwork Network { get; set; }

        public PipeNetMapComp MapComp => mapComp ??= parent?.Map?.GetComponent<PipeNetMapComp>();

        public int NetId { get; set; } = -1;

        public CompProperties_Pipe Props => (CompProperties_Pipe)props;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            if (Props.isPipe && parent.Graphic is not Graphic_LinkedPipe && parent.graphicInt is not null)
                parent.graphicInt = new Graphic_LinkedPipe(parent.graphicInt);

            foreach (var cell in parent.OccupiedRect())
            {
                foreach (var thing in cell.GetThingList(parent.Map).ToList())
                {
                    if (thing == parent) continue;
                    
                    var comp = thing.TryGetComp<CompPipe>();
                    if (comp == null || comp.Props.netTypeDef != Props.netTypeDef) continue;
                    thing.Destroy(DestroyMode.Deconstruct);
                }
            }

            MapComp.RegisterPipe(this);
        }
        
        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);

            MapComp.DeregisterPipe(this);
        }

        public override string CompInspectStringExtra()
        {
            if (!DebugSettings.godMode)
                return string.Empty;

            var builder = new StringBuilder();
            builder.AppendLine($"Net def: {this.GetId()}");
            builder.AppendLine($"Net ID: {NetId}");

            return builder.ToString().Trim();
        }
    }
}

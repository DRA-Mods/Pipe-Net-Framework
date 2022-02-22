using System.Collections.Generic;
using System.Text;
using PipeNetFramework.PipeNetResources;
using Verse;

namespace PipeNetFramework.Comps
{
    [HotSwappable]
    public abstract class CompPipeStorageBase : CompPipe
    {
        public abstract IEnumerable<PipeNetResourceDef> StoredThings();
        public abstract StorageContents GetContentsForThing(PipeNetResourceDef def);

        public abstract void Fill(ref float amount, PipeNetResourceDef thing);
        public abstract float Drain(float amount, PipeNetResourceDef thing);

        public override string CompInspectStringExtra()
        {
            var builder = new StringBuilder();

            foreach (var def in StoredThings())
            {
                var contents = GetContentsForThing(def);
                builder.AppendLine($"{def.label}: {contents.StoredVolume:f1}/{contents.MaxCapacity:f1}".CapitalizeFirst());
            }

            builder.AppendLine(base.CompInspectStringExtra());

            return builder.ToString().Trim();
        }

        public abstract class StorageContents : IExposable
        {
            public abstract float MaxCapacity { get; }
            public abstract float StoredVolume { get; set; }
            public virtual float EmptyVolume => MaxCapacity - StoredVolume;

            public abstract bool CanDrain { get; }
            public abstract bool CanFill { get; }

            public abstract void ExposeData();
        }
    }
}

using System;
using System.Collections.Generic;
using PipeNetFramework.PipeNetResources;
using Verse;

namespace PipeNetFramework.Comps
{
    public class CompProperties_PipeProducer : CompProperties_Pipe
    {
        public PipeNetResourceDef producedThing;
        public float producedCount;

        public CompProperties_PipeProducer() => compClass = typeof(CompPipeProducer);
        public CompProperties_PipeProducer(Type type) : base(type)
        { }

        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            foreach (var configError in base.ConfigErrors(parentDef))
                yield return configError;

            if (producedThing == null)
                yield return "Trying to produce a null thing";
            else if (!netTypeDef.storeableDefs.Contains(producedThing))
                yield return $"Trying to store {producedThing}, but it's not contained in the {netTypeDef}";

            if (producedCount <= 0)
                yield return $"Trying to produce {producedCount} amount of thing {producedThing} - it must be positive";
            else if (float.IsInfinity(producedCount))
                yield return $"Trying to produce infinite amount of thing {producedThing}";
            else if (float.IsNaN(producedCount))
                yield return $"Trying to produce nan amount of thing {producedThing}";
        }
    }
}

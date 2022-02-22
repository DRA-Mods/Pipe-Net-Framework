using System;
using System.Collections.Generic;
using PipeNetFramework.PipeNetResources;
using Verse;

namespace PipeNetFramework.Comps
{
    public class CompProperties_PipeConsumer : CompProperties_Pipe
    {
        public PipeNetResourceDef consumedThing;
        public float consumerCount;

        public CompProperties_PipeConsumer() => compClass = typeof(CompPipeConsumer);
        public CompProperties_PipeConsumer(Type type) : base(type)
        { }

        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            foreach (var configError in base.ConfigErrors(parentDef))
                yield return configError;

            if (consumedThing == null)
                yield return "Trying to produce a null thing";
            else if (!netTypeDef.storeableDefs.Contains(consumedThing))
                yield return $"Trying to store {consumedThing}, but it's not contained in the {netTypeDef}";

            if (consumerCount <= 0)
                yield return $"Trying to produce {consumerCount} amount of thing {consumedThing} - it must be positive";
            else if (float.IsInfinity(consumerCount))
                yield return $"Trying to produce infinite amount of thing {consumedThing}";
            else if (float.IsNaN(consumerCount))
                yield return $"Trying to produce nan amount of thing {consumedThing}";
        }
    }
}

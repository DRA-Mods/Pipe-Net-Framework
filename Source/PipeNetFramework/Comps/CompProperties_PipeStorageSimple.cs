using System;
using System.Collections.Generic;
using PipeNetFramework.PipeNetResources;
using Verse;

namespace PipeNetFramework.Comps
{
    public class CompProperties_PipeStorageSimple : CompProperties_PipeStorageBase
    {
        public PipeNetResourceDef storedThing;

        public float drainValue = 0.5F;

        public CompProperties_PipeStorageSimple() => compClass = typeof(CompPipeStorageSimple);

        public CompProperties_PipeStorageSimple(Type type) : base(type) 
        { }

        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            foreach (var configError in base.ConfigErrors(parentDef)) 
                yield return configError;

            if (storedThing == null)
                yield return "Trying to store a null thing";
            else if (!netTypeDef.storeableDefs.Contains(storedThing))
                yield return $"Trying to store {storedThing}, but it's not contained in the {netTypeDef}";

            if (drainValue < 0)
                drainValue = -drainValue;
        }
    }
}

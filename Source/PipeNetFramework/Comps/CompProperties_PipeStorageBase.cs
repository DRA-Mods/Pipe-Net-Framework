using System;
using System.Collections.Generic;
using Verse;

namespace PipeNetFramework.Comps
{
    public class CompProperties_PipeStorageBase : CompProperties_Pipe
    {
        public float maxCapacity;

        public CompProperties_PipeStorageBase()
        { }

        public CompProperties_PipeStorageBase(Type type) : base(type) 
        { }

        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            foreach (var configError in base.ConfigErrors(parentDef))
                yield return configError;

            if (maxCapacity < 0)
                maxCapacity = -maxCapacity;
            else if (maxCapacity == 0)
                yield return "Max capacity for storage cannot be 0";
        }
    }
}

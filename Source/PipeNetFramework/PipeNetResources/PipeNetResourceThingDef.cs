using System.Collections.Generic;
using Verse;

namespace PipeNetFramework.PipeNetResources
{
    public class PipeNetResourceThingDef : PipeNetResourceDef
    {
        public ThingDef content;

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (var configError in base.ConfigErrors())
                yield return configError;

            if (content == null)
                yield return $"{nameof(content)} cannot be null";
        }
    }
}

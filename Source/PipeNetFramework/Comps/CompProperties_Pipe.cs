using System;
using PipeNetFramework.DefOfs;
using PipeNetFramework.PipeNet;
using Verse;

namespace PipeNetFramework.Comps
{
    public class CompProperties_Pipe : CompProperties
    {
        /// <summary>
        /// A <see cref="PipeNetTypeDef"/> that represents which pipe net this comp belongs too.
        /// If <see langword="null"/> it's treated as a basic/universal/shared one.
        /// </summary>
        public PipeNetTypeDef netTypeDef;

        /// <summary>
        /// Used for pipe and pipe-like objects when building. If it's <see langword="true"/> then it won't be able to replace any non-pipe object.
        /// </summary>
        public bool isPipe = false;

        public CompProperties_Pipe() => compClass = typeof(CompPipe);
        public CompProperties_Pipe(Type type) => compClass = type;

        public override void ResolveReferences(ThingDef parentDef)
        {
            base.ResolveReferences(parentDef);
            netTypeDef ??= PipeNetTypeDefOf.PNF_UniversalNetwork;
        }
    }
}

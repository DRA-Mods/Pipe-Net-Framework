using PipeNetFramework.PipeNet;
using RimWorld;

namespace PipeNetFramework.DefOfs
{
    [DefOf]
    public static class PipeNetTypeDefOf
    {
        static PipeNetTypeDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(PipeNetTypeDefOf));

        public static PipeNetTypeDef PNF_UniversalNetwork;
    }
}

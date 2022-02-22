using PipeNetFramework.MapComps;

namespace PipeNetFramework.PipeNet
{
    public static class PipeNetTypeDefExtensions
    {
        public static string GetId(this PipeNetTypeDef def) => def?.defName ?? PipeNetMapComp.NullNetName;
    }
}

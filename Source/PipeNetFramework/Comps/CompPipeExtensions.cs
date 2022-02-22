using PipeNetFramework.MapComps;

namespace PipeNetFramework.Comps
{
    public static class CompPipeExtensions
    {
        public static string GetId(this CompPipe pipe) => pipe.Props.netTypeDef?.defName ?? PipeNetMapComp.NullNetName;
    }
}

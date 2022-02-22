using PipeNetFramework.MapComps;

namespace PipeNetFramework.PipeNet
{
    public static class PipeNetworkExtensions
    {
        public static string GetId(this PipeNetwork net) => net.NetTypeDef?.defName ?? PipeNetMapComp.NullNetName;
    }
}

using HarmonyLib;
using PipeNetFramework.Comps;
using PipeNetFramework.Overlay;
using RimWorld;
using Verse;

namespace PipeNetFramework.HarmonyPatches
{
    internal static class Designators
    {
        [HarmonyPatch(typeof(Designator_Build), nameof(Designator_Build.SelectedUpdate))]
        private static class Build_SelectedUpdate
        {
            public static void Postfix(BuildableDef ___entDef)
            {
                if (___entDef is ThingDef thingDef && thingDef.HasComp(typeof(CompPipe)))
                    SectionLayer_PipeNetwork.DrawGasPipeOverlayThisFrame();
            }
        }

        [HarmonyPatch(typeof(Designator_Install), nameof(Designator_Install.SelectedUpdate))]
        public static class Install_SelectedUpdate
        {
            public static void Postfix(Designator_Install __instance)
            {
                if (__instance.PlacingDef is ThingDef thingDef && thingDef.HasComp(typeof(CompPipe)))
                    SectionLayer_PipeNetwork.DrawGasPipeOverlayThisFrame();
            }
        }
    }
}

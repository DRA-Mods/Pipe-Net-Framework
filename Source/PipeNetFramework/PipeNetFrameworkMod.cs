using HarmonyLib;
using JetBrains.Annotations;
using Multiplayer.API;
using Verse;

namespace PipeNetFramework
{
    [UsedImplicitly]
    public class PipeNetFrameworkMod : Mod
    {
        public PipeNetFrameworkMod(ModContentPack content) : base(content)
        {
            if (MP.enabled) MP.RegisterAll();
            new Harmony("Dra.PipeNetFrameworkMod").PatchAll();
        }
    }
}
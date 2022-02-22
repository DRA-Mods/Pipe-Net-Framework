using System.Collections.Generic;
using PipeNetFramework.Overlay;
using PipeNetFramework.PipeNet;
using Verse;

namespace PipeNetFramework.Utilities
{
    [StaticConstructorOnStartup]
    public class ModGraphics
    {
        static ModGraphics()
        {
            foreach (var def in DefDatabase<PipeNetTypeDef>.AllDefs)
            {
                if (def.defName != null && def.overlayAtlasPath != null)
                    PipeOverlays[def.defName] = new Graphic_LinkedPipeOverlay(
                        GraphicDatabase.Get<Graphic_Single>(def.overlayAtlasPath, ShaderDatabase.MetaOverlay));
            }
        }

        public static readonly Dictionary<string, Graphic_LinkedPipeOverlay> PipeOverlays = new();
    }
}

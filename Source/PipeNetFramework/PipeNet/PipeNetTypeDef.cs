using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using PipeNetFramework.PipeNetResources;
using Verse;

namespace PipeNetFramework.PipeNet
{
    public class PipeNetTypeDef : Def
    {
        public Type pipeNetworkType;

        public virtual ConstructorInfo PipeNetworkConstructor { get; private set; }

        public List<PipeNetResourceDef> storeableDefs = new();

        public string overlayAtlasPath = null;

        public override void PostLoad()
        {
            base.PostLoad();

            if (pipeNetworkType != null)
                PipeNetworkConstructor = AccessTools.Constructor(pipeNetworkType, new[] { typeof(int), typeof(Map), typeof(PipeNetTypeDef), });
        }
    }
}

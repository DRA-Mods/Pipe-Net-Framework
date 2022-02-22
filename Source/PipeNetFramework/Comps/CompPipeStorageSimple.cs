using System.Collections.Generic;
using JetBrains.Annotations;
using Multiplayer.API;
using PipeNetFramework.Gizmos;
using PipeNetFramework.PipeNetResources;
using UnityEngine;
using Verse;

namespace PipeNetFramework.Comps
{
    [HotSwappable]
    internal class CompPipeStorageSimple : CompPipeStorageBase
    {
        private StoredContentsSimple stored;
        public virtual StoredContentsSimple Stored => stored ??= new StoredContentsSimple(this);

        public override IEnumerable<PipeNetResourceDef> StoredThings() { yield return Props.storedThing; }

        public override StorageContents GetContentsForThing(PipeNetResourceDef def)
        {
            if (def != Props.storedThing)
                return null;
            return Stored;
        }

        public new CompProperties_PipeStorageSimple Props => (CompProperties_PipeStorageSimple)props;

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Deep.Look(ref stored, nameof(stored));
            if (stored != null)
                stored.parent = this;
        }

        public override void CompTick() => Tick();
        public override void CompTickLong() => Tick();
        public override void CompTickRare() => Tick();

        public virtual void Tick()
        {
            if (Stored.isDraining)
            {
                var toDrain = Props.drainValue;
                toDrain = Mathf.Min(toDrain, Stored.StoredVolume);
                var undrained = toDrain;
                Network.Fill(ref undrained, Props.storedThing);

                Stored.StoredVolume -= toDrain - undrained;
                if (!Stored.CanDrain)
                    Stored.isDraining = false;
            }
        }

        public override void Fill(ref float amount, PipeNetResourceDef thing)
        {
            if (thing != Props.storedThing) return;

            var maxAdded = Stored.EmptyVolume;

            var toFill = Mathf.Min(amount, maxAdded);
            Stored.StoredVolume += toFill;
            amount -= toFill;
        }

        public override float Drain(float amount, PipeNetResourceDef thing)
        {
            if (thing != Props.storedThing) return 0f;

            var maxDrained = Stored.StoredVolume;

            var toDrain = Mathf.Min(amount, maxDrained);
            Stored.StoredVolume -= toDrain;
            return toDrain;
        }

        public class StoredContentsSimple : StorageContents
        {
            internal CompPipeStorageSimple parent;

            private float storedVolume;
            public bool isDraining = false;

            [UsedImplicitly]
            public StoredContentsSimple()
            { }

            public StoredContentsSimple(CompPipeStorageSimple parent)
                => this.parent = parent;

            public override float MaxCapacity => parent.Props.maxCapacity;

            public override float StoredVolume
            {
                get => storedVolume;
                set => storedVolume = value;
            }

            public override bool CanDrain => StoredVolume > 0;
            public override bool CanFill => StoredVolume < MaxCapacity - Mathf.Epsilon && !isDraining;

            public override void ExposeData()
            {
                Scribe_Values.Look(ref storedVolume, nameof(storedVolume));
                Scribe_Values.Look(ref isDraining, nameof(isDraining));
            }
        }

        #region Gizmos
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (var gizmo in base.CompGetGizmosExtra())
                yield return gizmo;

            if (Props.drainValue != 0)
                yield return SetDrain;
            if (Prefs.DevMode)
            {
                yield return DebugFill;
                yield return DebugEmpty;
            }
        }

        [SyncMethod]
        public virtual void ToggleDrain(int index)
        {
            if (Props.drainValue == 0)
                Stored.isDraining = false;
            else
                Stored.isDraining = !Stored.isDraining;
        }

        public virtual MultiCheckboxState IsActive(int index) => Stored.isDraining ? MultiCheckboxState.On : MultiCheckboxState.Off;

        [SyncMethod(debugOnly = true)]
        protected virtual void DebugFillMethod(int index) => Stored.StoredVolume = Stored.MaxCapacity;

        [SyncMethod(debugOnly = true)]
        protected virtual void DebugEmptyMethod(int index) => Stored.StoredVolume = 0;

        private Command_ToggleMulti setDrain;
        private Command_ActionMulti debugFill;
        private Command_ActionMulti debugEmpty;

        public Command_ToggleMulti SetDrain => setDrain ??= new Command_ToggleMulti
        {
            toggleAction = ToggleDrain,
            isActive = IsActive,
            defaultLabel = "PNF_DrainPipe".Translate(string.Empty).Trim(),
            defaultDesc = "PNF_DrainPipeDesc".Translate(),
        };

        public Command_ActionMulti DebugFill => debugFill ??= new Command_ActionMulti
        {
            action = DebugFillMethod,
            defaultLabel = "Dev: Fill",
        };

        public Command_ActionMulti DebugEmpty => debugEmpty ??= new Command_ActionMulti
        {
            action = DebugEmptyMethod,
            defaultLabel = "Dev: Empty",
        };

        #endregion
    }
}

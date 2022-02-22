using System.Collections.Generic;
using System.Linq;
using PipeNetFramework.Comps;
using PipeNetFramework.PipeNetResources;
using UnityEngine;
using Verse;

namespace PipeNetFramework.PipeNet
{
    [HotSwappable]
    public class PipeNetwork
    {
        public int Id { get; }
        /// <summary>
        /// The <see cref="Map"/> of this PipeNet.
        /// </summary>
        public Map Map { get; }
        /// <summary>
        /// The def (or null) of the net type
        /// </summary>
        public PipeNetTypeDef NetTypeDef { get; }

        public List<CompPipe> ConnectedPipes { get; } = new();

        public Dictionary<PipeNetResourceDef, float> MaxCapacity { get; } = new();
        public Dictionary<PipeNetResourceDef, float> StoredVolume { get; } = new();
        public Dictionary<PipeNetResourceDef, float> EmptyVolume { get; } = new();

        public PipeNetwork(int id, Map map, PipeNetTypeDef netTypeDef)
        {
            Id = id;
            Map = map;
            NetTypeDef = netTypeDef;

            if (netTypeDef?.storeableDefs != null)
            {
                foreach (var def in netTypeDef?.storeableDefs)
                    MaxCapacity[def] = 0f;
            }
        }

        public virtual void NetworkTick()
        {
            if (NetTypeDef == null) return;
            
            StoredVolume.Clear();
            EmptyVolume.Clear();

            foreach (var def in NetTypeDef.storeableDefs)
            {
                StoredVolume[def] = 0f;
                EmptyVolume[def] = 0f;
            }

            foreach (var pipe in ConnectedPipes)
            {
                if (pipe is not CompPipeStorageBase storage) continue;

                foreach (var def in storage.StoredThings())
                {
                    var capacity = storage.GetContentsForThing(def);

                    StoredVolume[def] += capacity.StoredVolume;
                    EmptyVolume[def] += capacity.EmptyVolume;
                }
            }
        }

        /// <summary>
        /// Adds a new <see cref="CompPipe"/> to this network
        /// </summary>
        /// <param name="pipe">The <see cref="CompPipe"/> or its subtype to add to this network.</param>
        public virtual void RegisterPipe(CompPipe pipe)
        {
            if (pipe?.parent == null || pipe.parent.Map != Map) return;

            pipe.NetId = Id;
            pipe.Network = this;
            ConnectedPipes.AddDistinct(pipe);

            if (pipe is CompPipeStorageBase storage && NetTypeDef != null)
            {
                foreach (var def in storage.StoredThings())
                {
                    if (MaxCapacity.ContainsKey(def)) 
                        MaxCapacity[def] += storage.GetContentsForThing(def).MaxCapacity;
                }
            }
        }

        /// <summary>
        /// Removes an existing <see cref="PipeNetwork"/> from this network
        /// </summary>
        /// <param name="pipe"></param>
        public virtual void DeregisterPipe(CompPipe pipe)
        {
            if (pipe == null) return;

            ConnectedPipes.Remove(pipe);

            if (pipe is CompPipeStorageBase storage && NetTypeDef != null)
            {
                foreach (var def in storage.StoredThings())
                {
                    if (MaxCapacity.ContainsKey(def))
                    {
                        MaxCapacity[def] -= storage.GetContentsForThing(def).MaxCapacity;
                        if (MaxCapacity[def] < 0)
                            MaxCapacity[def] = 0;
                    }
                }
            }
        }

        public virtual void Fill(float value, PipeNetResourceDef thing) => Fill(ref value, thing);

        public virtual void Fill(ref float value, PipeNetResourceDef thing)
        {
            if (!EmptyVolume.TryGetValue(thing, out var tmp) || tmp <= 0) return;
            if (!NetTypeDef.storeableDefs.Any(x => x == thing)) return;
            
            var fillable = ConnectedPipes
                .OfType<CompPipeStorageBase>()
                .Where(x => x.GetContentsForThing(thing)?.CanFill == true)
                .ToArray();

            var tries = fillable.Length * 10;

            var valueToTryFill = 0f;
            for (var i = 0; i < tries; i++)
            {
                if (i % fillable.Length == 0)
                    valueToTryFill = value / fillable.Length;

                var attempt = valueToTryFill;
                fillable[i % fillable.Length].Fill(ref attempt, thing);
                var filled = valueToTryFill - attempt;
                value -= filled;

                StoredVolume[thing] += filled;
                EmptyVolume[thing] -= filled;

                if (value <= Mathf.Epsilon || EmptyVolume[thing] <= 0)
                    break;
            }
        }

        public virtual bool TryDrain(float value, PipeNetResourceDef thing)
        {
            var drained = Drain(value, thing);
            if (drained >= value - Mathf.Epsilon) return true;

            Fill(ref drained, thing);
            return false;
        }

        public virtual float Drain(float value, PipeNetResourceDef thing)
        {
            if (!StoredVolume.TryGetValue(thing, out var tmp) || tmp <= 0) return 0f;
            if (!NetTypeDef.storeableDefs.Any(x => x == thing)) return 0f;
            
            var drainable = ConnectedPipes
                .OfType<CompPipeStorageBase>()
                .Where(x => x.GetContentsForThing(thing)?.CanDrain == true)
                .ToArray();

            var tries = drainable.Length * 10;

            var drainedTotal = 0f;
            var valueToTryDrain = 0f;
            for (var i = 0; i < tries; i++)
            {
                if (i % drainable.Length == 0)
                    valueToTryDrain = value / drainable.Length;

                var drained = drainable[i % drainable.Length].Drain(valueToTryDrain, thing);
                drainedTotal += drained;

                StoredVolume[thing] -= drained;
                EmptyVolume[thing] += drained;

                if (drainedTotal >= value - Mathf.Epsilon || StoredVolume[thing] <= 0)
                    break;
            }

            return drainedTotal;
        }

        public virtual void CleanNet()
        {
            ConnectedPipes.RemoveAll(x => x == null || !x.parent.Spawned);
            ConnectedPipes.RemoveDuplicates();
        }

        /// <summary>
        /// Merges this <see cref="PipeNetwork"/> with another, moving all the elements from the other one into this one.
        /// </summary>
        /// <param name="other">The network that will be merged into this one, and all its elements cleared (it should).</param>
        /// <returns><see langword="true"/> if successful.</returns>
        public virtual bool MergeNets(PipeNetwork other, int[] grid)
        {
            if (other == this)
            {
                Log.Error("Trying to merge PipeNet with itself!");
                return false;
            }

            if (other.Map != Map)
            {
                Log.Error("Trying to merge two PipeNets on different maps!");
                return false;
            }

            if (other.NetTypeDef != NetTypeDef)
            {
                Log.Error($"Trying to merge two PipeNets of a different type! Trying to merge {other.NetTypeDef?.ToString() ?? "null"} into {NetTypeDef?.ToString() ?? "null"}");
                return false;
            }

            ConnectedPipes.AddRange(other.ConnectedPipes);

            other.ConnectedPipes.ForEach(p =>
            {
                p.NetId = Id;
                p.Network = this;

                foreach (var pos in p.parent.OccupiedRect())
                    grid[Map.cellIndices.CellToIndex(pos)] = Id;
            });

            other.ConnectedPipes.Clear();
            CleanNet();

            return true;
        }

        /// <summary>
        /// Removes all elements from this net.
        /// </summary>
        /// <param name="grid">The grid to remove grid IDs of connected things from.</param>
        public virtual void RemoveAll(int[] grid)
        {
            if (grid != null)
            {
                foreach (var pipe in ConnectedPipes)
                {
                    foreach (var cell in pipe.parent.OccupiedRect()) 
                        grid[Map.cellIndices.CellToIndex(cell)] = -1;
                }
            }

            ConnectedPipes.Clear();
        }
    }
}
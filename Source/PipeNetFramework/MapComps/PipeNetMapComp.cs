using System.Collections.Generic;
using System.Linq;
using PipeNetFramework.Comps;
using PipeNetFramework.PipeNet;
using Verse;

namespace PipeNetFramework.MapComps
{
    [HotSwappable]
    public class PipeNetMapComp : MapComponent
    {
        public const string NullNetName = "PNF_UniversalNetwork";

        public Dictionary<string, Dictionary<int, PipeNetwork>> Networks { get; } = new();
        public Dictionary<int, PipeNetwork> this[string name]
        {
            get => GetOrCreatePipeNetList(name ?? NullNetName);
            private set => Networks[name ?? NullNetName] = value;
        }

        public Dictionary<int, PipeNetwork> this[PipeNetTypeDef def]
        {
            get => GetOrCreatePipeNetList(def);
            set => Networks[def?.defName ?? NullNetName] = value;
        }

        public Dictionary<string, int[]> Grids { get; } = new();

        public PipeNetMapComp(Map map) : base(map)
        { }

        public override void MapComponentTick()
        {
            base.MapComponentTick();

            foreach (var net in Networks.Values.SelectMany(netList => netList.Values))
                net.NetworkTick();
        }

        public virtual void RegisterPipe(CompPipe pipe)
        {
            var id = pipe.GetId();
            var pipeNets = this[id];
            var grid = GetOrCreatePipeGrid(id);

            var rect = pipe.parent.OccupiedRect();
            var netId = rect.Select(c => grid[map.cellIndices.CellToIndex(c)]).FirstOrFallback(i => i >= 0, -1);
            if (netId >= 0)
            {
                pipeNets[netId].RegisterPipe(pipe);
                RegisterGridFor(pipe, netId, grid);
                return;
            }

            var neighboring = GenAdj
                .CellsAdjacentCardinal(pipe.parent)
                .Select(c => grid[map.cellIndices.CellToIndex(c)])
                .Where(x => x != -1)
                .Distinct()
                .ToArray();

            switch (neighboring.Length)
            {
                case 0:
                    {
                        netId = 0;
                        if (pipeNets.Count > 0)
                            netId = pipeNets.Values.Max(x => x.Id) + 1;

                        PipeNetwork net;
                        if (pipe.Props.netTypeDef?.PipeNetworkConstructor != null)
                            net = (PipeNetwork)pipe.Props.netTypeDef.PipeNetworkConstructor.Invoke(new object[] { netId, pipe.parent.Map, pipe.Props.netTypeDef });
                        else
                            net = new PipeNetwork(netId, pipe.parent.Map, pipe.Props.netTypeDef);

                        net.RegisterPipe(pipe);
                        pipeNets.Add(net.Id, net);
                        break;
                    }
                case 1:
                    {
                        if (pipeNets.Count == 0) goto case 0;

                        var net = pipeNets[neighboring[0]];
                        net.RegisterPipe(pipe);
                        netId = net.Id;
                        break;
                    }
                default:
                    {
                        if (pipeNets.Count == 0) goto case 0;
                        if (pipeNets.Count == 1) goto case 1;

                        var net = pipeNets[neighboring[0]];
                        net.RegisterPipe(pipe);
                        RegisterGridFor(pipe, net.Id, grid);

                        for (var i = 1; i < neighboring.Length; i++)
                        {
                            netId = neighboring[i];
                            net.MergeNets(pipeNets[netId], grid);
                            pipeNets.Remove(netId);
                        }

                        netId = net.Id;

                        break;
                    }
            }

            RegisterGridFor(pipe, netId, grid);
        }

        public virtual void DeregisterPipe(CompPipe pipe)
        {
            var id = pipe.GetId();
            pipe.Network?.DeregisterPipe(pipe);

            RegisterAll(id);
        }

        protected virtual void RegisterAll(string id)
        {
            var pipeNets = this[id];
            var pipes = pipeNets
                .Values
                .SelectMany(x => x.ConnectedPipes)
                .ToArray();

            Grids.Remove(id);
            Networks[id].Clear();

            foreach (var p in pipes)
                RegisterPipe(p);
        }

        public virtual Dictionary<int, PipeNetwork> GetOrCreatePipeNetList(PipeNetTypeDef def) => GetOrCreatePipeNetList(def?.defName ?? NullNetName);

        public virtual Dictionary<int, PipeNetwork> GetOrCreatePipeNetList(string id)
        {
            if (Networks.TryGetValue(id, out var net))
                return net;
            return this[id] = new Dictionary<int, PipeNetwork>();
        }

        public virtual int[] GetOrCreatePipeGrid(PipeNetTypeDef def) => GetOrCreatePipeGrid(def?.defName ?? NullNetName);

        public virtual int[] GetOrCreatePipeGrid(string id)
        {
            if (!Networks.ContainsKey(id))
                return null;

            if (Grids.TryGetValue(id, out var grid))
                return grid;

            var cells = Grids[id] = Enumerable.Repeat(-1, map.cellIndices.NumGridCells).ToArray();
            return cells;
        }

        public PipeNetwork PipeNetAt(IntVec3 cell, PipeNetTypeDef def)
        {
            var id = def?.defName ?? NullNetName;

            var gridIndex = map.cellIndices.CellToIndex(cell);
            if (gridIndex < 0) return null;

            var index = GetOrCreatePipeGrid(id)?[gridIndex];
            if (index is null or < 0) return null;

            var netList = this[id];
            if (netList is null || !netList.ContainsKey(index.Value)) return null;

            return netList[index.Value];
        }

        private void RegisterGridFor(ThingComp comp, int pipeNetId, PipeNetTypeDef def) => RegisterGridFor(comp.parent, pipeNetId, def);

        private void RegisterGridFor(Thing thing, int pipeNetId, PipeNetTypeDef def) => RegisterGridFor(thing, pipeNetId, def.GetId());

        private void RegisterGridFor(ThingComp comp, int pipeNetId, string id) => RegisterGridFor(comp.parent, pipeNetId, id);

        private void RegisterGridFor(Thing thing, int pipeNetId, string id) => RegisterGridFor(thing, pipeNetId, GetOrCreatePipeGrid(id));

        private void RegisterGridFor(ThingComp comp, int pipeNetId, int[] grid) => RegisterGridFor(comp.parent, pipeNetId, grid);

        private void RegisterGridFor(Thing thing, int pipeNetId, int[] grid)
        {
            foreach (var cell in thing.OccupiedRect())
                grid[map.cellIndices.CellToIndex(cell)] = pipeNetId;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ParksComputing.Cells;

/// <summary>
/// Immutable compiled mesh of ICell nodes.
/// Supports fan-out and fan-in by counting expected inputs and buffering for fan-in nodes.
/// </summary>
public sealed class Mesh<TIn, TOut> : IRunnable<TIn, TOut> {
    private readonly ICellBox[] _nodes;
    private readonly Dictionary<int, List<int>> _edges;
    private readonly int _root;
    private readonly int _sink;

    internal Mesh(
        ICellBox[] nodes,
        Dictionary<int, List<int>> edges,
        int root,
        int sink) {
        _nodes = nodes;
        _edges = edges;
        _root = root;
        _sink = sink;
    }

    public async Task<TOut> RunAsync(TIn seed, CancellationToken ct = default) {
        var queue = new Queue<(int nodeId, object? value)>();
        var buffers = new Dictionary<int, List<object?>>();
        var inCount = ComputeInDegree(_edges, _nodes.Length);
        object? final = null;

        var executed = new HashSet<int>();

        queue.Enqueue((_root, seed!));

        while (queue.Count > 0) {
            ct.ThrowIfCancellationRequested();
            var (id, val) = queue.Dequeue();
            var node = _nodes[id];

            if (inCount[id] > 1) {
                if (!buffers.TryGetValue(id, out var list)) {
                    buffers[id] = list = new List<object?>();
                }

                list.Add(val);
                if (list.Count < inCount[id])
                    continue;

                object? outVal = null;
                foreach (var v in list) {
                    outVal = await node.RunAsync(v, ct);
                }

                buffers.Remove(id);
                Propagate(id, outVal);
            }
            else {
                var outVal = await node.RunAsync(val, ct);
                Propagate(id, outVal);
            }

            executed.Add(id);
        }

        // Now call CompleteAsync for all executed nodes, *after* the full run
        foreach (var id in executed) {
            await _nodes[id].CompleteAsync(ct);
        }

        return (TOut)final!;

        void Propagate(int node, object? outVal) {
            if (node == _sink) {
                final = outVal;
            }

            if (_edges.TryGetValue(node, out var kids)) {
                foreach (var c in kids) {
                    queue.Enqueue((c, outVal));
                }
            }
        }
    }


    private static int[] ComputeInDegree(
        Dictionary<int, List<int>> edges, int n) {
        var inDegree = new int[n];
        foreach (var kv in edges)
            foreach (int to in kv.Value)
                inDegree[to]++;
        return inDegree;
    }
}

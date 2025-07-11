namespace ParksComputing.Cells;

/// <summary>Immutable, compiled DAG.</summary>
public sealed class Mesh<TIn, TOut> : IRunnable<TIn, TOut> {
    private readonly ICellBox[] _nodes;
    private readonly Dictionary<int, List<int>> _edges;
    private readonly int _rootId;
    private readonly int _sinkId;

    internal Mesh(
        ICellBox[] nodes,
        Dictionary<int, List<int>> edges,
        int rootId,
        int sinkId)
        => (_nodes, _edges, _rootId, _sinkId) = (nodes, edges, rootId, sinkId);

    public async Task<TOut> RunAsync(TIn seed, CancellationToken ct = default) {
        var valueAtNode = new object?[_nodes.Length];

        valueAtNode[_rootId] = seed!;
        var ready = new Queue<int>();
        ready.Enqueue(_rootId);

        while (ready.Count > 0) {
            int id = ready.Dequeue();
            var input = valueAtNode[id];

            var output = await _nodes[id].RunAsync(input, ct);
            valueAtNode[id] = output;

            if (!_edges.TryGetValue(id, out var outs))
                continue;

            foreach (int to in outs) {
                valueAtNode[to] = output; // simple overwrite; fan-in merge could go here
                ready.Enqueue(to);
            }
        }

        return (TOut?)valueAtNode[_sinkId]
            ?? throw new InvalidOperationException("Mesh produced null.");
    }
}

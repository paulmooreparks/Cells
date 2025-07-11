namespace ParksComputing.Cells;

/// <summary>Immutable compiled mesh.</summary>
public sealed class Mesh<TResult> {
    private readonly ICellBox[] _nodes;
    private readonly Dictionary<int, List<int>> _edges;
    private readonly int _root;

    internal Mesh(
        ICellBox[] nodes,
        Dictionary<int, List<int>> edges,
        int rootNode) => (_nodes, _edges, _root) = (nodes, edges, rootNode);

    public async Task<TResult> RunAsync(object? seed, CancellationToken ct = default) {
        var ready = new Queue<int>();
        var pendingInputs = new Dictionary<int, object?>();
        pendingInputs[_root] = seed;
        ready.Enqueue(_root);

        while (ready.Count > 0) {
            int id = ready.Dequeue();
            var input = pendingInputs[id];
            var output = await _nodes[id].RunAsync(input, ct);

            pendingInputs[id] = output;   // keep final node's result

            if (!_edges.TryGetValue(id, out var outs)) {
                continue;
            }

            foreach (var to in outs) {
                // for fan-in you could merge/aggregate; here we overwrite
                pendingInputs[to] = output;
                ready.Enqueue(to);
            }
        }

        return (TResult?)pendingInputs[_nodes.Length - 1]
            ?? throw new InvalidOperationException("Mesh produced null");
    }
}

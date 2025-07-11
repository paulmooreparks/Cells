namespace ParksComputing.Cells;

/// <summary>Fluent builder for an acyclic mesh (DAG).</summary>
public sealed class MeshBuilder {
    private readonly List<ICellBox> _nodes = [];
    private readonly Dictionary<int, List<int>> _edges = new(); // from ⇒ [to]

    public static MeshBuilder Create() => new();

    /// <returns>The index of the added node.</returns>
    public int Add<TIn, TOut>(ICell<TIn, TOut> cell) {
        int id = _nodes.Count;
        _nodes.Add(new CellBox<TIn, TOut>(cell));
        return id;
    }

    public MeshBuilder Connect(int fromNode, int toNode) {
        if (!_edges.TryGetValue(fromNode, out var list))
            _edges[fromNode] = list = [];
        list.Add(toNode);
        return this;
    }

    /// <param name="rootNode">Entry node that receives the seed.</param>
    /// <param name="sinkNode">Node whose <c>Out</c> is returned by <c>RunAsync</c>.</param>
    public Mesh<TIn, TOut> Build<TIn, TOut>(int rootNode, int sinkNode)
        => new(_nodes.ToArray(), _edges, rootNode, sinkNode);

    /// <summary>
    /// Convenience overload: assume the node added last is the sink.
    /// </summary>
    public Mesh<TIn, TOut> Build<TIn, TOut>(int rootNode)
        => new(_nodes.ToArray(), _edges, rootNode, _nodes.Count - 1);
}

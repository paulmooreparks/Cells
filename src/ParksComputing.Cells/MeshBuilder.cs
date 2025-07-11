namespace ParksComputing.Cells;

/// <summary>
/// Fluent builder for an acyclic mesh (DAG).
/// Every cell’s <c>Out</c> feeds zero-or-many downstream <c>In</c>s.
/// Execution is topologically sorted; each cell runs once.
/// </summary>
public sealed class MeshBuilder {
    private readonly List<ICellBox> _nodes = [];
    private readonly Dictionary<int, List<int>> _edges = new();  // from → [to,…]

    public static MeshBuilder Create() => new();

    /// <returns>index that identifies the node inside the mesh</returns>
    public int Add<TIn, TOut>(ICell<TIn, TOut> cell) {
        int id = _nodes.Count;
        _nodes.Add(new CellBox<TIn, TOut>(cell));
        return id;
    }

    public MeshBuilder Connect(int fromNode, int toNode) {
        if (!_edges.TryGetValue(fromNode, out var list)) {
            _edges[fromNode] = list = new();
        }

        list.Add(toNode);

        return this;
    }

    public Mesh<TResult> Build<TResult>(int rootNode) => 
        new(_nodes.ToArray(), _edges, rootNode);
}

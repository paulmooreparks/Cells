namespace ParksComputing.Cells;

/// <summary>
/// Opaque handle to a node whose cell has input TIn and output TOut.
/// </summary>
public readonly struct MeshKey<TIn, TOut> {
    internal int Key { get; }
    internal MeshKey(int key) => Key = key;
}

public sealed class MeshBuilder {
    private readonly List<ICellBox> _nodes = new();
    private readonly Dictionary<int, List<int>> _edges = new();

    private MeshBuilder() { }
    public static MeshBuilder Create() => new();

    /// <summary>
    /// Add a cell whose In-type is TIn and Out-type is TOut.
    /// Returns a MeshKey&lt;TIn,TOut&gt; you can use for Connect/Build.
    /// </summary>
    public MeshKey<TIn, TOut> Add<TIn, TOut>(ICell<TIn, TOut> cell) {
        var key = new MeshKey<TIn, TOut>(_nodes.Count);
        _nodes.Add(new CellBox<TIn, TOut>(cell));
        return key;
    }

    /// <summary>
    /// Connect the output of `from` into the input of `to`.
    /// Compile-time safe: ensures from.TOut == to.TIn.
    /// </summary>
    public MeshBuilder Connect<TFromIn, TMid, TTo>(
        MeshKey<TFromIn, TMid> from,
        MeshKey<TMid, TTo> to) {
        if (!_edges.TryGetValue(from.Key, out var list))
            _edges[from.Key] = list = new();
        list.Add(to.Key);
        return this;
    }

    /// <summary>
    /// Build a strongly-typed Mesh&lt;TRootIn, TResult&gt; that starts
    /// at `root` (which must accept TRootIn) and ends at `sink`
    /// (which must produce TResult).
    /// </summary>
    public Mesh<TRootIn, TResult> Build<TRootIn, TRootOut, TResultIn, TResult>(
        MeshKey<TRootIn, TRootOut> root,
        MeshKey<TResultIn, TResult> sink)
        => new(
            nodes: _nodes.ToArray(),
            edges: _edges,
            root: root.Key,
            sink: sink.Key
        );
}

namespace ParksComputing.Cells;
public sealed class PathwayBuilder<TStart, TCurrent> {
    private readonly List<ICellBox> _steps;
    internal PathwayBuilder(List<ICellBox> steps) => _steps = steps;

    // add a typed cell, auto-boxed
    public PathwayBuilder<TStart, TNext> Then<TNext>(ICell<TCurrent, TNext> cell) {
        _steps.Add(new CellBox<TCurrent, TNext>(cell));
        return new(_steps);
    }

    // quick λ-map without writing a class
    public PathwayBuilder<TStart, TNext> Map<TNext>(Func<TCurrent, TNext> map)
        => Then(new DelegateCell<TCurrent, TNext>(map));

    // finish
    public CompiledPathway<TStart, TCurrent> Build()
        => new([.. _steps]);
}

public sealed class CompiledPathway<TStart, TEnd> {
    private readonly ICellBox[] _steps;
    internal CompiledPathway(ICellBox[] steps) => _steps = steps;

    public async Task<TEnd> RunAsync(TStart input, CancellationToken ct = default) {
        object? current = input!;
        foreach (var s in _steps) {
            current = await s.RunAsync(current, ct);
        }
        return (TEnd)current!;
    }
}

public sealed class DelegateCell<TIn, TOut> : ICell<TIn, TOut> {
    private readonly Func<TIn, TOut> _map;
    public DelegateCell(Func<TIn, TOut> map) => _map = map;
    public string Name => nameof(DelegateCell<TIn, TOut>);
    public TIn In { get; set; } = default!;
    public TOut Out { get; private set; } = default!;
    public Task ExecuteAsync(CancellationToken _) {
        Out = _map(In);
        return Task.CompletedTask;
    }
}

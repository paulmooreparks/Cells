namespace ParksComputing.Cells.Testing;

public sealed class StubCell<TIn, TOut> : ICell<TIn, TOut> {
    private readonly Func<TIn, TOut> _func;

    public StubCell(TOut staticResult) => _func = _ => staticResult;
    public StubCell(Func<TIn, TOut> func) => _func = func;
    public StubCell(Exception exToThrow) => _func = _ => throw exToThrow;

    public string Name => nameof(StubCell<TIn, TOut>);
    public TIn In { get; set; } = default!;
    public TOut Out { get; private set; } = default!;

    public Task ExecuteAsync(CancellationToken _) {
        Out = _func(In);
        return Task.CompletedTask;
    }
}

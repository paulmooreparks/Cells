namespace ParksComputing.Cells;

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

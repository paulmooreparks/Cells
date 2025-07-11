namespace ParksComputing.Cells;

public sealed class CellBox<TIn, TOut> : ICellBox {
    private readonly ICell<TIn, TOut> _cell;
    public CellBox(ICell<TIn, TOut> cell) => _cell = cell;

    public async ValueTask<object?> RunAsync(object? input, CancellationToken ct) {
        _cell.In = (TIn)input!;
        await _cell.ExecuteAsync(ct);
        return _cell.Out!;
    }
}

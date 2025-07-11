namespace ParksComputing.Cells;

public interface ICell<TIn, out TOut> {
    TIn In { get; set; }
    TOut Out { get; }
    Task ExecuteAsync(CancellationToken ct = default);
}

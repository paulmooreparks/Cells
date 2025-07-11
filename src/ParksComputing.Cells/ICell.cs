namespace ParksComputing.Cells;

public interface ICell<TIn, out TOut> {
    string Name { get; } 
    TIn In { get; set; }
    TOut Out { get; }
    Task ExecuteAsync(CancellationToken ct = default);
}

namespace ParksComputing.Cells.Testing;

public sealed class CaptureTapCell<T> : ICell<T, T> {
    private readonly List<T> _capture = new();
    public IReadOnlyList<T> Captured => _capture;

    public string Name => nameof(CaptureTapCell<T>);
    public T In { get; set; } = default!;
    public T Out => In;

    public Task ExecuteAsync(CancellationToken _) {
        _capture.Add(In);
        return Task.CompletedTask;
    }
}

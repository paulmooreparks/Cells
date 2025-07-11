namespace ParksComputing.Cells;

public sealed class CompiledPathway<TStart, TEnd> : IRunnable<TStart, TEnd> {
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

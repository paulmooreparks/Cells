namespace ParksComputing.Cells;

public sealed class CompiledPathway<TStart, TEnd> : IRunnable<TStart, TEnd> {
    private readonly ICellBox[] _steps;
    internal CompiledPathway(ICellBox[] steps) => _steps = steps;

    public async Task<TEnd> RunAsync(TStart input, CancellationToken ct = default) {
        object? current = input!;
        foreach (var step in _steps) {
            // 1) Run the cell on the current value
            current = await step.RunAsync(current, ct);

            // 2) Immediately notify it that its output has been fully enqueued
            //    (so it can reset any per-run state)
            await step.CompleteAsync(ct);
        }
        return (TEnd)current!;
    }
}

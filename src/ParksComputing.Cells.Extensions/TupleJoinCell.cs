namespace ParksComputing.Cells.Extensions;

// (string, string[]) join
public sealed class TupleJoinCell<TA, TB> : ICell<object?, (TA, TB)> {
    public string Name => nameof(TupleJoinCell<TA, TB>);
    public object? In { get; set; }

    public (TA, TB) Out { get; private set; }

    private TA? _a;
    private TB? _b;
    private bool _ready;   // ensures we emit only once

    public Task ExecuteAsync(CancellationToken _ = default) {
        switch (In) {
            case TA a:
                _a = a;
                break;
            case TB b:
                _b = b;
                break;
            default:
                throw new InvalidOperationException($"Join cell received unsupported type {In?.GetType().Name}");
        }

        if (_a is not null && _b is not null && !_ready) {
            Out = (_a, _b);
            _ready = true;          // emit only this first complete tuple
        }
        // If still missing a half, keep previous Out unchanged
        return Task.CompletedTask;
    }
}

namespace ParksComputing.Cells.Extensions;

public sealed class SafeCell<TIn, TOut> : ICell<TIn, Result<TOut, Exception>> {
    private readonly ICell<TIn, TOut> _inner;

    public SafeCell(ICell<TIn, TOut> inner)
        => _inner = inner ?? throw new ArgumentNullException(nameof(inner));

    public required TIn In { get; set; }

    // Out is *always* assigned in the ctor (dummy value) and again in ExecuteAsync.
    private static readonly Result<TOut, Exception> _notRun =
        Result<TOut, Exception>.Failure(new InvalidOperationException("Cell not executed yet."));

    public Result<TOut, Exception> Out { get; private set; } = _notRun;

    public string Name => $"{nameof(SafeCell<TIn,TOut>)}({_inner.Name})";

    public async Task ExecuteAsync(CancellationToken ct = default) {
        // Defensive check for reference-type inputs that might be set to null
        if (typeof(TIn).IsClass && In is null) {
            throw new NullReferenceException(nameof(In));
        }

        _inner.In = In;

        try {
            await _inner.ExecuteAsync(ct).ConfigureAwait(false);
            Out = Result<TOut, Exception>.Success(_inner.Out);
        }
        catch (Exception ex) {
            Out = Result<TOut, Exception>.Failure(ex);
        }
    }
}

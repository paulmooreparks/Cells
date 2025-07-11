namespace ParksComputing.Cells.Extensions;

/// <summary>
///  Collapses <c>N</c> parallel branches back into a single list,
///  preserving successes and failures exactly as they arrive.
/// </summary>
public sealed class FanInCell<T>
        : ICell<IReadOnlyList<Result<T, Exception>>,
                IReadOnlyList<Result<T, Exception>>> {
    public string Name => nameof(FanInCell<T>);

    public IReadOnlyList<Result<T, Exception>> In { get; set; }
        = Array.Empty<Result<T, Exception>>();

    public IReadOnlyList<Result<T, Exception>> Out { get; private set; }
        = Array.Empty<Result<T, Exception>>();

    public Task ExecuteAsync(CancellationToken ct = default) {
        // identity – just forward
        Out = In;
        return Task.CompletedTask;
    }
}

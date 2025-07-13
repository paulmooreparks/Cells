namespace ParksComputing.Cells;

/// <summary>
/// Fans the same input into N branches in parallel and collects
/// <c>BooleanResult&lt;TOut2,Exception&gt;</c> from every branch.
/// </summary>
public sealed class FanOutCell<TIn, TOut> : ICell<TIn, IReadOnlyList<Result<TOut, Exception>>> {
    private readonly ICell<TIn, TOut>[] _branches;

    public FanOutCell(params ICell<TIn, TOut>[] branches) => _branches = branches;

    public string Name => "FanOut(" + string.Join(',', _branches.Select(b => b.Name)) + ")";

    public TIn In { get; set; } = default!;
    public IReadOnlyList<Result<TOut, Exception>> Out { get; private set; } = [];

    public async Task ExecuteAsync(CancellationToken ct = default) {
        var tasks = new Task<Result<TOut, Exception>>[_branches.Length];

        for (int i = 0; i < _branches.Length; ++i) {
            tasks[i] = RunBranchAsync(_branches[i], ct);
        }

        Out = await Task.WhenAll(tasks);
    }

    private async Task<Result<TOut, Exception>> RunBranchAsync(ICell<TIn, TOut> cell, CancellationToken ct) {
        try {
            // each branch sees the same input
            cell.In = In;
            await cell.ExecuteAsync(ct);
            return Result<TOut, Exception>.Success(cell.Out);
        }
        catch (Exception ex) {
            return Result<TOut, Exception>.Failure(ex);
        }
    }
}
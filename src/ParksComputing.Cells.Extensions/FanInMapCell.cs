namespace ParksComputing.Cells.Extensions;

/// <summary>
/// Takes a batch of <c>Result&lt;TIn,Exception&gt;</c>.
/// • For every <strong>successful</strong> item
///     1. runs a single <paramref name="preMap"/> cell (<c>TIn → TMid</c>)
///     2. passes the mapped value to a <see cref="FanOutCell{TMid,TOut}"/>
///        built by <paramref name="branchFactory"/> (<c>TMid → TOut</c> in N branches)
/// • Preserves any failures from the input batch
/// • Collects <strong>all</strong> successes &amp; failures (flattened) in <see cref="Out"/>
/// </summary>
public sealed class FanInMapCell<TIn, TMid, TOut>
       : ICell<IReadOnlyList<Result<TIn, Exception>>,
               IReadOnlyList<Result<TOut, Exception>>> {
    private readonly ICell<TIn, TMid> _preMap;                       // e.g. HTML→text
    private readonly Func<TIn, ICell<TMid, TOut>[]> _branchFactory;  // builds next fan-out

    public FanInMapCell(ICell<TIn, TMid> preMap,
                        Func<TIn, ICell<TMid, TOut>[]> branchFactory) {
        _preMap = preMap;
        _branchFactory = branchFactory;
    }

    public string Name => nameof(FanInMapCell<TIn,TMid,TOut>);

    public IReadOnlyList<Result<TIn, Exception>> In { get; set; } = [];
    public IReadOnlyList<Result<TOut, Exception>> Out { get; private set; } = [];

    public async Task ExecuteAsync(CancellationToken ct = default) {
        var all = new List<Result<TOut, Exception>>();

        foreach (var res in In) {
            // propagate upstream failure unchanged
            if (!res.IsOk) {
                all.Add(Result<TOut, Exception>.Failure(res.Err));
                continue;
            }

            try {
                // ─── 1. single transform (TIn ➜ TMid) ─────────────────────
                _preMap.In = res.Ok;
                await _preMap.ExecuteAsync(ct);
                var mid = _preMap.Out;

                // ─── 2. fan-out to N branches in parallel (TMid ➜ TOut) ───
                var fanOut = new FanOutCell<TMid, TOut>(_branchFactory(res.Ok));
                fanOut.In = mid;
                await fanOut.ExecuteAsync(ct);

                // successes & failures from this level
                all.AddRange(fanOut.Out);
            }
            catch (Exception ex) {
                // catch failure in preMap or fanOut construction
                all.Add(Result<TOut, Exception>.Failure(ex));
            }
        }

        Out = all;
    }
}

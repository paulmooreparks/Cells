using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParksComputing.Cells;
internal sealed class SwitchCell<T> : ICell<T, T> {
    private readonly Func<T, bool> _predicate;
    private readonly IReadOnlyDictionary<bool, ICell<T, T>> _branches;

    public SwitchCell(Func<T, bool> predicate,
                      IReadOnlyDictionary<bool, ICell<T, T>> branches)
        => (_predicate, _branches) = (predicate, branches);

    public string Name => nameof(SwitchCell<T>);
    public T In { get; set; } = default!;
    public T Out { get; private set; } = default!;

    public async Task ExecuteAsync(CancellationToken ct = default) {
        bool key = _predicate(In);
        if (!_branches.TryGetValue(key, out var branch))
            throw new InvalidOperationException($"No branch for {key}");

        branch.In = In;
        await branch.ExecuteAsync(ct);
        Out = branch.Out;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParksComputing.Cells.Samples.FileWatcher.Cells;

public sealed class CollectMatchesCell : ICell<ConditionalResult<string>, List<string>> {
    private readonly List<string> _list = [];
    public ConditionalResult<string> In { get; set; }
    public List<string> Out => _list;
    public string Name => nameof(CollectMatchesCell);

    public Task ExecuteAsync(CancellationToken _ = default) {
        if (In.BooleanResult) {
            Console.WriteLine($"Collecting match: {In.Out}");
            _list.Add(In.Out);
        } else {
            Console.WriteLine("No match collected because BooleanResult is false.");
        }

        return Task.CompletedTask;
    }

    public Task CompletedAsync(CancellationToken _ = default) {
        Console.WriteLine($"CollectMatchesCell completed with {Out.Count} matches collected.");
        _list.Clear();
        return Task.CompletedTask;
    }
}

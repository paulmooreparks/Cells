using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ParksComputing.Cells;

namespace DocInsight;

internal class DocInsightMainCell : ICell<string[], int> {
    public string[] In { get; set; } = [];
    public int Out { get; }

    public Task ExecuteAsync(CancellationToken ct = default) {
        throw new NotImplementedException();
    }
}

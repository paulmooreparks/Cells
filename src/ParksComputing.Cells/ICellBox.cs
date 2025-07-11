using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParksComputing.Cells;

public interface ICellBox {
    ValueTask<object?> RunAsync(object? input, CancellationToken ct);
}

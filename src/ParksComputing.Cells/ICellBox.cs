using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParksComputing.Cells;

public interface ICellBox {
    /// <summary>Run once with boxed input, produce boxed output.</summary>
    ValueTask<object?> RunAsync(object? input, CancellationToken ct);

    /// <summary>
    /// Forward the CompletedAsync call to the underlying ICell.
    /// Called by the mesh after propagation.
    /// </summary>
    ValueTask CompleteAsync(CancellationToken ct);
    object Cell { get; }
}

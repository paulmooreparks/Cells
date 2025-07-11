using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParksComputing.Cells;

public interface IRunnable<TIn, TOut> {
    Task<TOut> RunAsync(TIn input, CancellationToken ct = default);
}

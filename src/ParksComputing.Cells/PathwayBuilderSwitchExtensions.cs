using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParksComputing.Cells;

public static class PathwayBuilderSwitchExtensions {
    public static ISwitchBuilder<TStart, TCurrent> Switch<TStart, TCurrent>(
        this PathwayBuilder<TStart, TCurrent> builder,
        Func<TCurrent, bool> predicate)
        => new SwitchBuilder<TStart, TCurrent>(builder, predicate);
}

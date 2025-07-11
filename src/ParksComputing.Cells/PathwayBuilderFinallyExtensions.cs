using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParksComputing.Cells;

public static class PathwayBuilderFinallyExtensions {
    // single-generic version: caller writes .Finally<int>()
    public static CompiledPathway<TStart, TEnd> Finally<TStart, TEnd>(
        this PathwayBuilder<TStart, TEnd> builder)
        => builder.Build();
}
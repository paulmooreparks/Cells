namespace ParksComputing.Cells;

public sealed class PathwayBuilder<TStart, TCurrent> {
    private readonly List<ICellBox> _steps;
    internal PathwayBuilder(List<ICellBox> steps) => _steps = steps;

    // add a typed cell, auto-boxed
    public PathwayBuilder<TStart, TNext> Then<TNext>(ICell<TCurrent, TNext> cell) {
        _steps.Add(new CellBox<TCurrent, TNext>(cell));
        return new(_steps);
    }

    // quick λ-map without writing a class
    public PathwayBuilder<TStart, TNext> Map<TNext>(Func<TCurrent, TNext> map)
        => Then(new DelegateCell<TCurrent, TNext>(map));

    // finish
    public CompiledPathway<TStart, TCurrent> Build()
        => new([.. _steps]);
}

namespace ParksComputing.Cells;

public sealed class PathwayBuilder<TStart, TCurrent> {
    private readonly List<ICellBox> _steps;
    internal PathwayBuilder(List<ICellBox> steps) => _steps = steps;

    // no change to your fluent API
    public PathwayBuilder<TStart, TNext> Then<TNext>(ICell<TCurrent, TNext> cell) {
        _steps.Add(new CellBox<TCurrent, TNext>(cell));
        return new PathwayBuilder<TStart, TNext>(_steps);
    }

    public PathwayBuilder<TStart, TNext> Map<TNext>(Func<TCurrent, TNext> map)
        => Then(new DelegateCell<TCurrent, TNext>(map));

    // your existing Build
    public CompiledPathway<TStart, TCurrent> Build()
        => new CompiledPathway<TStart, TCurrent>(_steps.ToArray());
}

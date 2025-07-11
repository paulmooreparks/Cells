namespace ParksComputing.Cells;

public static class Pathway {
    public static PathwayBuilder<TStart, TStart> Start<TStart>()
        => new([]);
}


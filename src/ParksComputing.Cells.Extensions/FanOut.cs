namespace ParksComputing.Cells.Extensions;

public static class FanOut {
    // run N parallel branches returning BooleanResult<TOut,Exception>[]
    public static PathwayBuilder<TStart, IReadOnlyList<Result<TOut, Exception>>>
        ToAll<TStart, TMid, TOut>(
            this PathwayBuilder<TStart, TMid> b,
            params ICell<TMid, TOut>[] branches)
        => b.Then(new FanOutCell<TMid, TOut>(branches));
}

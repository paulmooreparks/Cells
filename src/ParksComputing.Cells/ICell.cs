namespace ParksComputing.Cells;

public interface ICell<TIn, out TOut> {
    string Name { get; } 
    TIn In { get; set; }
    TOut Out { get; }

    /// <summary>
    /// Executes the cell's processing logic asynchronously, transforming the input data to output data.
    /// </summary>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ExecuteAsync(CancellationToken ct = default);

    /// <summary>
    /// Called by the cell runner **after** this cell has:
    ///   • run ExecuteAsync for all inputs (fan-in),  
    ///   • enqueued its Out to all downstream cells.
    /// Use this to clear or reset any accumulated state.
    /// Default is a no-op.
    /// </summary>
    Task CompleteAsync(CancellationToken ct = default)
        => Task.CompletedTask;    // C#8+ default interface method
}

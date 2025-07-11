namespace ParksComputing.Cells.Hosting;

public static class CellsHost {
    public static async Task<int> RunAsync<TMain>(string[] args, CancellationToken ct = default)
        where TMain : ICell<string[], int>, new() {
        var main = new TMain { In = args };
        await main.ExecuteAsync(ct);
        return main.Out;
    }
}

using ParksComputing.Cells.Hosting;

namespace DocInsight;

class Program {
    static async Task Main(string[] args) {
        await CellsHost.RunAsync<DocInsightMainCell>(args);
    }
}

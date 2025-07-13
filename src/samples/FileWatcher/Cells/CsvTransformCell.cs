using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text.Json;
using ParksComputing.Cells;

namespace ParksComputing.Cells.Samples.FileWatcher.Cells;

public sealed class CsvTransformCell : ICell<FileSeed, string> {
    public string Name => nameof(CsvTransformCell);
    public FileSeed In { get; set; } = default!;
    public string Out { get; private set; } = string.Empty;

    public async Task ExecuteAsync(CancellationToken ct = default) {
        Console.WriteLine($"Transforming CSV file: {In.FullPath}...");
        using var reader = new StreamReader(In.FullPath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));

        var records = csv.GetRecords<dynamic>().ToList();
        Out = JsonSerializer.Serialize(records);
        Console.WriteLine($"CSV transformation completed. Output: {Out ?? "null"}");
        await Task.CompletedTask;
    }
}

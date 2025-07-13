using System.Text.Json;

using ParksComputing.Cells;

namespace ParksComputing.Cells.Samples.FileWatcher.Cells;

public sealed class JsonTransformCell : ICell<FileSeed, string> {
    private static readonly JsonSerializerOptions _opts = new() {
        WriteIndented = false,
        PropertyNameCaseInsensitive = true
    };

    public string Name => nameof(JsonTransformCell);
    public FileSeed In { get; set; } = default!;
    public string Out { get; private set; } = string.Empty;

    public async Task ExecuteAsync(CancellationToken ct = default) {
        Console.WriteLine($"[{Name}] Transforming JSON file: {In.FullPath}");
        var json = await File.ReadAllTextAsync(In.FullPath, ct);

        // quick validation / canonicalisation
        var doc = JsonDocument.Parse(json);
        Out = JsonSerializer.Serialize(doc.RootElement, _opts);
        Console.WriteLine($"JSON transformation completed. Output: {Out ?? "null"}");
    }
}

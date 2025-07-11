using ParksComputing.Cells;
using ParksComputing.Cells.Extensions;
using ParksComputing.Xfer.Lang;

using System.Text.Json;

namespace ParksComputing.Cells.Samples.FileWatcher.Cells;

/// <summary>
/// Converts a <c>.xfer</c> file into JSON text (lossless round-trip).
/// </summary>
public sealed class XferTransformCell : ICell<FileSeed, string> {
    public string Name => nameof(XferTransformCell);

    public FileSeed In { get; set; } = default!;
    public string Out { get; private set; } = string.Empty;

    public async Task ExecuteAsync(CancellationToken ct = default) {
        // 1. load file
        string xferText = await File.ReadAllTextAsync(In.FullPath, ct);
        var obj = XferConvert.Deserialize<TestRecord>(xferText);
        Out = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = false });
    }

    public class TestRecord {
        public string foo { get; set; } = string.Empty;
    }
}

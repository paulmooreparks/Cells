using System.IO;

using ParksComputing.Cells;

namespace ParksComputing.Cells.Samples.FileWatcher.Cells;

/// <summary>Sets <see cref="FileSeed.Mime"/> based on file extension.</summary>
public sealed class MimeSnifferCell : ICell<FileSeed, FileSeed> {
    private static readonly Dictionary<string, string> _map = new(StringComparer.OrdinalIgnoreCase) {
        [".csv"] = "text/csv",
        [".json"] = "application/json",
        [".xml"] = "application/xml",
        [".xfer"] = "application/xfer"
    };

    public string Name => nameof(MimeSnifferCell);
    public FileSeed In { get; set; } = default!;
    public FileSeed Out { get; private set; } = default!;

    public Task ExecuteAsync(CancellationToken _ = default) {
        var ext = Path.GetExtension(In.FullPath);
        var mime = _map.TryGetValue(ext, out var m) ? m : "application/octet-stream";
        Out = In with { Mime = mime };
        return Task.CompletedTask;
    }
}

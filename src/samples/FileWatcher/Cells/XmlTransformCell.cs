using System.Xml.Linq;

using ParksComputing.Cells;

namespace ParksComputing.Cells.Samples.FileWatcher.Cells;

public sealed class XmlTransformCell : ICell<FileSeed, string> {
    public string Name => nameof(XmlTransformCell);
    public FileSeed In { get; set; } = default!;
    public string Out { get; private set; } = string.Empty;

    public async Task ExecuteAsync(CancellationToken ct = default) {
        Console.WriteLine($"[{Name}] Transforming XML file: {In.FullPath}");
        var xmlText = await File.ReadAllTextAsync(In.FullPath, ct);
        var doc = XDocument.Parse(xmlText);

        // canonicalise: remove insignificant whitespace, normalise end-of-line
        Out = doc.ToString(SaveOptions.DisableFormatting);
        Console.WriteLine($"XML transformation completed. Output: {Out ?? "null"}");
    }
}

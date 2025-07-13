using System.Globalization;

using ParksComputing.Cells;

namespace ParksComputing.Cells.Samples.FileWatcher.Cells;

/// <summary>
/// Persists the incoming JSON payload to ./uploads/YYYYMMDD_HHMMSSfff.json
/// and echoes the file path as <see cref="Out"/>.
/// </summary>
public sealed class UploadCell(string folder) : ICell<List<string>, string> {
    public string Name => nameof(UploadCell);

    public List<string> In { get; set; } = [];

    public string Out { get; private set; } = string.Empty;

    public async Task ExecuteAsync(CancellationToken ct = default) {
        if (In == null || In.Count == 0 || !In.All(item => !string.IsNullOrWhiteSpace(item))) {
            Console.WriteLine("UploadCell skipped: In is null, empty or contains null/empty strings.");
            return;
        }

        if (string.IsNullOrWhiteSpace(folder)) {
            throw new ArgumentException("Folder path cannot be null or empty.", nameof(folder));
        }

        if (!Directory.Exists(folder)) {
            try {
                Directory.CreateDirectory(folder);
                Console.WriteLine($"Created directory: {folder}");
            } catch (Exception ex) {
                Console.WriteLine($"Error creating directory: {ex.Message}");
                return;
            }
        }

        if (!Path.IsPathRooted(folder)) {
            folder = Path.GetFullPath(folder);
        }

        if (!Directory.Exists(folder)) {
            throw new DirectoryNotFoundException($"The specified folder does not exist: {folder}");
        }

        Console.WriteLine($"Saving payload to {folder}...");

        // Join all strings in In to form the JSON payload
        // Assuming In contains JSON strings, we can concatenate them
        foreach (var item in In) {
            if (string.IsNullOrWhiteSpace(item)) {
                Console.WriteLine("Skipping null or empty string in In.");
                continue;
            }

            string payload = string.Join(Environment.NewLine, item);
            string file = Path.Combine(folder, $"{DateTime.UtcNow:yyyyMMdd_HHmmssfff}.json");

            try {
                await File.WriteAllTextAsync(file, payload, ct);
                Out = file;
                Console.WriteLine($"Saved {file}");
            }
            catch (Exception ex) {
                Console.WriteLine($"Error saving file: {ex.Message}");
            }
        }
    }
}

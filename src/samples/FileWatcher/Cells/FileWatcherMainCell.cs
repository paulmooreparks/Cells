using System.Collections.Concurrent;
using System.IO;
using System.Threading.Channels;

using Microsoft.Extensions.Logging;

using ParksComputing.Cells;
using ParksComputing.Cells.Hosting;
using ParksComputing.Cells.Samples.FileWatcher;

namespace ParksComputing.Cells.Samples.FileWatcher.Cells;

/// <summary>
/// Long-running service: watches a folder, pushes <see cref="FileSeed"/>s
/// through <see cref="FileWatcherMesh"/> until cancellation.
/// Returns <see cref="AppResult"/> when gracefully stopped.
/// </summary>
public sealed class FileWatcherMainCell : ICell<WatchOptions, AppResult> {
    private readonly ILogger<FileWatcherMainCell> _log;
    private Mesh<FileSeed, string>? _mesh;            // build later

    public FileWatcherMainCell(ILogger<FileWatcherMainCell> log)
        => _log = log ?? throw new ArgumentNullException(nameof(log));

    public string Name => nameof(FileWatcherMainCell);

    public WatchOptions In { get; set; } = default!;

    public AppResult Out { get; private set; } = new(0, "not started");

    public async Task ExecuteAsync(CancellationToken ct = default) {
        // Build mesh the first time ExecuteAsync is called, when In is set
        _mesh ??= FileWatcherMesh.Build(In.UploadFolder);

        var chan = Channel.CreateBounded<FileSeed>(
            new BoundedChannelOptions(100) { SingleReader = false, SingleWriter = true });

        using var fsw = new FileSystemWatcher(In.WatchFolder) {
            EnableRaisingEvents = true,
            IncludeSubdirectories = false
        };

        fsw.Created += (_, e) =>
               _ = Task.Run(async () => {
                   // wait until the file can be opened for reading
                   await WaitForFileReadyAsync(e.FullPath, ct);
                   chan.Writer.TryWrite(new FileSeed(e.FullPath, ""));
               }, CancellationToken.None);

        var workers = Enumerable.Range(0, Environment.ProcessorCount)
                                .Select(_ => Worker(ct)).ToArray();

        try {
            await Task.WhenAll(workers);
            Out = new AppResult(0, "watcher stopped");
        }
        catch (OperationCanceledException) {
            Out = new AppResult(0, "cancelled");
        }

        async Task Worker(CancellationToken token) {
            await foreach (var seed in chan.Reader.ReadAllAsync(token)) {
                try {
                    await _mesh.RunAsync(seed, token);
                }
                catch (Exception ex) {
                    _log.LogError(ex, "Pipeline failed for {File}", seed.FullPath);
                }
            }
        }
    }

    private static async Task WaitForFileReadyAsync(string path, CancellationToken ct) {
        const int maxAttempts = 10;
        for (int i = 0; i < maxAttempts; i++) {
            try {
                // try opening with shared read so we don't block readers
                using var fs = File.Open(path,
                                          FileMode.Open,
                                          FileAccess.Read,
                                          FileShare.Read);
                if (fs.Length > 0)
                    return;    // success
            }
            catch (IOException) {
                // still locked—wait and retry
            }
            await Task.Delay(200, ct);
        }
        // Give up after a few tries; let the pipeline pick it up later (or log an error)
    }
}

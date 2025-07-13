using FluentAssertions;
using ParksComputing.Cells.Samples.FileWatcher;
using ParksComputing.Cells.Samples.FileWatcher.Cells;
using Microsoft.Extensions.Logging.Abstractions;

namespace ParksComputing.Cells.Tests;

public class FileWatcherMainCellTests {
    [Fact(Timeout = 10_000)]
    public async Task Processes_Csv_File_End_to_End() {
        // temp folders
        string watchFolder = Path.Combine(Path.GetTempPath(), "watch-" + Guid.NewGuid());
        string uploadFolder = Path.Combine(Path.GetTempPath(), "upload-" + Guid.NewGuid());
        Directory.CreateDirectory(watchFolder);
        Directory.CreateDirectory(uploadFolder);

        // options & cell
        var options = new WatchOptions(watchFolder, uploadFolder);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(8));

        var cell = new FileWatcherMainCell(new NullLogger<FileWatcherMainCell>()) {
            In = options
        };

        // run watcher
        var runTask = cell.ExecuteAsync(cts.Token);

        // drop CSV into the watch folder
        string csvPath = Path.Combine(watchFolder, "sample.csv");
        await File.WriteAllTextAsync(csvPath, "id,name\n1,Paul");

        // poll the upload folder for a .json result (max 5 s)
        bool found = false;
        for (int i = 0; i < 50 && !found; i++) {
            found = Directory.EnumerateFiles(uploadFolder, "*.json").Any();
            if (!found) {
                await Task.Delay(100, cts.Token);
            }
        }

        // stop the watcher
        cts.Cancel();
        await runTask;

        found.Should().BeTrue("a JSON file should be written to the upload folder");
        cell.Out.ExitCode.Should().Be(0);
    }
}

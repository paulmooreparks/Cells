using System.Runtime.Versioning;

using Microsoft.Extensions.Logging;

using ParksComputing.Cells.Hosting;
using ParksComputing.Cells.Samples.FileWatcher;
using ParksComputing.Cells.Samples.FileWatcher.Cells;

// 1. Quick-and-dirty arg parse 
if (args.Length is 0 || args[0] is "-h" or "--help") {
    Console.WriteLine("""
        Usage:
            dotnet run -- watch <folder-to-monitor> [--serve]

            --serve   Also start the local upload API (future feature).
        """);
    return 1;
}

string watchFolder = Path.GetFullPath(args[0]);
string uploadFolder = Path.GetFullPath(args[1]);

if (!Directory.Exists(watchFolder)) {
    Console.Error.WriteLine($"Watch folder '{watchFolder}' does not exist.");
    return 1;
}

if (!Directory.Exists(uploadFolder)) {
    Console.Error.WriteLine($"Upload folder '{uploadFolder}' does not exist.");
    return 1;
}

var options = new WatchOptions(watchFolder, uploadFolder);

// 2. Logging (simple console) 
using ILoggerFactory lf = LoggerFactory.Create(b => {
    b.SetMinimumLevel(LogLevel.Information)
     .AddSimpleConsole(c => { c.SingleLine = true; });
});

var logger = lf.CreateLogger<FileWatcherMainCell>();

// 3. Run the main cell until Ctrl-C 
using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

var mainCell = new FileWatcherMainCell(logger) { In = options };

try {
    await mainCell.ExecuteAsync(cts.Token);
}
catch (OperationCanceledException) {
    // graceful shutdown
}

AppResult result = mainCell.Out;
Console.WriteLine(result.Message);

return result.ExitCode;

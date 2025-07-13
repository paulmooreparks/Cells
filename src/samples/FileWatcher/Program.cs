using System;
using System.IO;
using Microsoft.Extensions.Logging;
using ParksComputing.Cells;
using ParksComputing.Cells.Samples.FileWatcher;
using ParksComputing.Cells.Samples.FileWatcher.Cells;
using ParksComputing.Cells.Hosting; // for AppResult

var program = Pathway
    .Start<string[]>()
    .Then(new ArgParseCell<WatchOptions>( 
        parser: args => {
            if (args.Length != 2 ||
                args[0] is "-h" or "--help")
                throw new ArgumentException();
            return new WatchOptions(
                Path.GetFullPath(args[0]),
                Path.GetFullPath(args[1])
            );
        },
        usageText: """
            Usage:
                filewatcher <folder-to-monitor> <upload-folder>
            """
    ))
    .Then(new ValidateDirectoryCell<WatchOptions>(
        opts => opts.WatchFolder, description: "Watch folder"
    ))
    .Then(new ValidateDirectoryCell<WatchOptions>(
        opts => opts.UploadFolder, description: "Upload folder"
    ))
    .Then(new LoggingSetupCell<WatchOptions, FileWatcherMainCell>(builder =>
          builder.SetMinimumLevel(LogLevel.Information)
                 .AddSimpleConsole(c => c.SingleLine = true)
     ))
    .Then(new CancellationCell<WatchOptions, FileWatcherMainCell>())
    .Then(new RunnerCell<WatchOptions, FileWatcherMainCell, AppResult>(
          async (log, opts, token) => {
              var main = new FileWatcherMainCell(log) { In = opts };
              await main.ExecuteAsync(token);
          },
          (code, msg) => new AppResult(code, msg)
      ))
    .Map(result =>
    {
        Console.WriteLine(result.Message);
        return result.ExitCode;
    })
    .Finally();

int exitCode = await program.RunAsync(args);
Environment.Exit(exitCode);

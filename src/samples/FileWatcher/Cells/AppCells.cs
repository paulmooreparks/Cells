
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using ParksComputing.Cells;
using ParksComputing.Cells.Hosting;  // for AppResult, if you have one

namespace ParksComputing.Cells.Samples.FileWatcher.Cells;
public sealed class ArgParseCell<TOptions> : ICell<string[], TOptions> {
    private readonly Func<string[], TOptions> _parser;
    private readonly string _usageText;

    public ArgParseCell(Func<string[], TOptions> parser, string usageText) {
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        _usageText = usageText ?? throw new ArgumentNullException(nameof(usageText));
    }

    public string Name => nameof(ArgParseCell<TOptions>);
    public string[] In { get; set; } = Array.Empty<string>();
    public TOptions Out { get; private set; } = default!;  // Initialize Out to avoid null reference issues

    public Task ExecuteAsync(CancellationToken ct = default) {
        try {
            Out = _parser(In);
        }
        catch (ArgumentException ex) {
            Console.Error.WriteLine(_usageText);
            throw new OperationCanceledException("Argument parsing failed", ex);
        }
        return Task.CompletedTask;
    }
}

// 2) Validate that a folder exists
public sealed class ValidateDirectoryCell<TOptions> : ICell<TOptions, TOptions> {
    private readonly Func<TOptions, string> _selector;
    private readonly string _description;

    public ValidateDirectoryCell(Func<TOptions, string> selector, string description) {
        _selector = selector ?? throw new ArgumentNullException(nameof(selector));
        _description = description ?? throw new ArgumentNullException(nameof(description));
    }

    public string Name => $"{nameof(ValidateDirectoryCell<TOptions>)}({_description})";
    public TOptions In { get; set; } = default!;  // Initialize In to avoid null reference issues
    public TOptions Out { get; private set; } = default!;  // Initialize Out to avoid null reference issues

    public Task ExecuteAsync(CancellationToken ct = default) {
        var path = _selector(In);
        if (!Directory.Exists(path))
            throw new DirectoryNotFoundException($"{_description} '{path}' does not exist.");
        Out = In;
        return Task.CompletedTask;
    }
}

// 3) Setup an ILoggerFactory and ILogger<TCategory>
/// <summary>
/// Takes a TIn (your options), sets up ILogger<TCategory>, 
/// and emits a tuple (Options, Logger).
/// </summary>
public sealed class LoggingSetupCell<TIn, TCategory>
    : ICell<TIn, (TIn Options, ILogger<TCategory> Logger)> {
    public string Name => nameof(LoggingSetupCell<TIn,TCategory>);
    public TIn In { get; set; } = default!;
    public (TIn Options, ILogger<TCategory> Logger) Out { get; private set; }

    private readonly Action<ILoggingBuilder> _configure;
    public LoggingSetupCell(Action<ILoggingBuilder> configure)
        => _configure = configure;

    public Task ExecuteAsync(CancellationToken _) {
        var lf = LoggerFactory.Create(_configure);
        var log = lf.CreateLogger<TCategory>();
        Out = (In, log);
        return Task.CompletedTask;
    }
}

/// <summary>
/// Takes (Options,Logger) and emits (Options,Logger,Token).
/// </summary>
public sealed class CancellationCell<TIn, TCategory>
    : ICell<(TIn Options, ILogger<TCategory> Logger),
            (TIn Options, ILogger<TCategory> Logger, CancellationToken Token)> {
    public string Name => nameof(CancellationCell<TIn, TCategory>);

    public (TIn Options, ILogger<TCategory> Logger) In { get; set; } = default!;

    public (TIn Options, ILogger<TCategory> Logger, CancellationToken Token) Out { get; private set; }

    public Task ExecuteAsync(CancellationToken _ = default) {
        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) => {
            e.Cancel = true;
            cts.Cancel();
        };
        Out = (In.Options, In.Logger, cts.Token);
        return Task.CompletedTask;
    }
}

public sealed class RunnerCell<TOptions, TCategory, TResult>
    : ICell<(TOptions Options, ILogger<TCategory> Logger, CancellationToken Token), TResult> {
    private readonly Func<ILogger<TCategory>, TOptions, CancellationToken, Task> _runner;
    private readonly Func<int, string, TResult> _makeResult;

    public RunnerCell(
        Func<ILogger<TCategory>, TOptions, CancellationToken, Task> runner,
        Func<int, string, TResult> makeResult) {
        _runner = runner;
        _makeResult = makeResult;
    }

    public string Name => nameof(RunnerCell<TOptions, TCategory, TResult>);

    public (TOptions Options, ILogger<TCategory> Logger, CancellationToken Token) In { get; set; } = default!;

    public TResult Out { get; private set; } = default!;

    public async Task ExecuteAsync(CancellationToken ct = default) {
        var (opts, log, token) = In;
        try {
            await _runner(log, opts, token);
            Out = _makeResult(0, "Success");
        }
        catch (OperationCanceledException) {
            Out = _makeResult(1, "Cancelled");
        }
        catch (Exception ex) {
            log.LogError(ex, "Unhandled error in RunnerCell");
            Out = _makeResult(1, ex.Message);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using ParksComputing.Cells;              // IRunnable, Mesh, CompiledPathway

namespace ParksComputing.Cells.Testing;

/// <summary>
/// Generic test harness for any Cells runtime artefact
/// (mesh *or* compiled pathway).
/// Captures timing and exceptions for later assertions.
/// </summary>
public sealed class TestHarness<TIn, TOut> {
    private readonly IRunnable<TIn, TOut> _runner;
    private readonly List<RunTrace<TIn, TOut>> _traces = new();

    public TestHarness(IRunnable<TIn, TOut> runner)
        => _runner = runner;

    /// Convenience overloads for common concrete types
    public TestHarness(Mesh<TIn, TOut> mesh) : this((IRunnable<TIn, TOut>)mesh) { }
    public TestHarness(CompiledPathway<TIn, TOut> pathway) : this((IRunnable<TIn, TOut>)pathway) { }

    public IReadOnlyList<RunTrace<TIn, TOut>> Traces => _traces;

    public async Task<TOut> RunAsync(TIn seed, CancellationToken ct = default) {
        var sw = Stopwatch.StartNew();
        try {
            var result = await _runner.RunAsync(seed, ct);
            _traces.Add(new(seed, result, sw.Elapsed, null));
            return result;
        }
        catch (Exception ex) {
            _traces.Add(new(seed, default!, sw.Elapsed, ex));
            throw;
        }
    }
}

public record RunTrace<TIn, TOut>(TIn Seed, TOut? Result, TimeSpan Duration, Exception? Error);

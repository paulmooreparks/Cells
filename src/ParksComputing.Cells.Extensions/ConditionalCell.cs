using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParksComputing.Cells.Extensions;

/// <summary>
/// Runs the wrapped cell only when <paramref name="predicate"/> is true.
/// Otherwise forwards the seed unchanged and flags <see cref="Fired"/> false.
/// </summary>
public sealed class ConditionalCell<TIn, TOut> : ICell<TIn, ConditionalResult<TOut>> {
    private readonly Func<TIn, bool> _predicate;
    private readonly ICell<TIn, TOut> _inner;

    internal ConditionalCell(ICell<TIn, TOut> inner, Func<TIn, bool> predicate) {
        _predicate = predicate;
        _inner = inner;
    }

    public TIn In { get => _inner.In; set => _inner.In = value; }
    public ConditionalResult<TOut> Out { get; internal set; }
    public ICell<TIn, TOut> Inner => _inner;

    public async Task ExecuteAsync(CancellationToken ct = default) {
        if (!_predicate(In)) {
            Out = new ConditionalResult<TOut>(_inner.Out, false);
            return;
        }

        await _inner.ExecuteAsync(ct);
        Out = new ConditionalResult<TOut>(_inner.Out, true);
    }

    public string Name => $"{nameof(ConditionalCell<TIn, TOut>)}({_inner.Name})";
}

public static class Conditional {
    /// <summary>
    /// Creates a conditional cell that executes the inner cell only if the predicate is true.
    /// </summary>
    public static ICell<TIn, ConditionalResult<TOut>> Create<TIn, TOut>(ICell<TIn, TOut> inner, Func<TIn, bool> predicate) {
        return new ConditionalCell<TIn, TOut>(inner, predicate);
    }
}

public struct ConditionalResult<TOut> {
    public TOut Out { get; }
    public bool BooleanResult { get; internal set; }
    public ConditionalResult(TOut outValue, bool boolValue) {
        Out = outValue;
        BooleanResult = boolValue;
    }
}
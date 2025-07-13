using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParksComputing.Cells.Extensions;

/// <summary>
/// Minimal “maybe” type: Either <c>Some(value)</c> or <c>None</c>.
/// Designed for pipeline cells that might emit a result.
/// </summary>
public readonly struct Option<T> {
    private readonly T? _value;
    public bool IsSome { get; }
    public bool IsNone => !IsSome;
    private Option(T value) { _value = value; IsSome = true; }
    public static Option<T> Some(T v) => new(v);
    public static Option<T> None() => new();
    public T ValueOr(T fallback) => IsSome ? _value! : fallback;
    public static implicit operator Option<T>(T value) => value is null ? None() : Some(value);
}


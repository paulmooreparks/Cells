namespace ParksComputing.Cells;

/// <summary>
/// Lightweight, allocation-cheap discriminated union (success or error).
/// </summary>
public readonly struct Result<TOk, TErr> {
    private readonly TOk _ok;
    private readonly TErr _err;
    public bool IsOk { get; }
    public TOk Ok => IsOk ? _ok : throw new InvalidOperationException();
    public TErr Err => IsOk ? throw new InvalidOperationException() : _err;

    private Result(bool ok, TOk okVal, TErr errVal)
        => (IsOk, _ok, _err) = (ok, okVal, errVal);

    public static Result<TOk, TErr> Success(TOk v) => new(true, v, default!);
    public static Result<TOk, TErr> Failure(TErr e) => new(false, default!, e);
}

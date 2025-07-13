namespace ParksComputing.Cells.Hosting;

/// <summary>
/// Standard result returned by a demo’s MainCell.
/// <paramref name="ExitCode"/> 0 = success; non-zero means error.
/// <paramref name="Message"/>  human-readable summary.
/// </summary>
public sealed record AppResult(int ExitCode, string Message);

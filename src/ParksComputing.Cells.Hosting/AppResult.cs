namespace ParksComputing.Cells.Hosting;

/// <summary>
/// Standard result returned by a demo’s MainCell.
/// <paramref name="ExitCode"/> 0 = success; non-zero means error.
/// <paramref name="Message"/>  human-readable summary.
/// </summary>
public class AppResult {
    public int ExitCode { get; }
    public string Message { get; }
    
    public AppResult(int exitCode, string message) {
        ExitCode = exitCode;
        Message = message;
    }
}

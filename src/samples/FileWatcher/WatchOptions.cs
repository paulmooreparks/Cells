namespace ParksComputing.Cells.Samples.FileWatcher;

/// <summary>
/// CLI / GUI options for the File-Watcher demo.
/// </summary>
/// <param name="WatchFolder">Absolute or relative path to monitor.</param>
/// <param name="UploadFolder">Absolute folder where uploaded files will be stored.</param>
public sealed record WatchOptions(
    string WatchFolder,
    string UploadFolder
    );

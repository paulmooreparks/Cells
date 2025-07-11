using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParksComputing.Cells.Extensions;

/// <summary>
/// Seed record produced by the folder-watch loop.  
/// <paramref name="FullPath"/> is the absolute file path;  
/// <paramref name="Mime"/> is the best-guess MIME string (e.g. "text/csv").
/// </summary>
public sealed record FileSeed(string FullPath, string Mime);

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAssertions;

using ParksComputing.Cells.Samples.FileWatcher.Cells;

namespace ParksComputing.Cells.Tests;

public class XferTransformCellTests {
    [Fact]
    public async Task Converts_SimpleDocument_ToJson() {
        var tmp = Path.GetTempFileName();
        await File.WriteAllTextAsync(tmp, 
"""
</ comment /> {foo "bar"}
""");

        var seed = new FileSeed(tmp, "application/xfer");
        var cell = new XferTransformCell { In = seed };

        await cell.ExecuteAsync();

        cell.Out.Should().Contain("\"foo\":\"bar\"");
    }
}

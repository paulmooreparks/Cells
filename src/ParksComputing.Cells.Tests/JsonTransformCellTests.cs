using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAssertions;

using ParksComputing.Cells.Samples.FileWatcher;
using ParksComputing.Cells.Samples.FileWatcher.Cells;

namespace ParksComputing.Cells.Tests;

public class JsonTransformCellTests {
    [Fact]
    public async Task Canonicalises_Json() {
        var tmp = Path.GetTempFileName();
        await File.WriteAllTextAsync(tmp, "{ \"id\" : 1 }");

        var seed = new FileSeed(tmp, "application/json");
        var cell = new JsonTransformCell { In = seed };

        await cell.ExecuteAsync();

        cell.Out.Should().Be("{\"id\":1}");
    }
}
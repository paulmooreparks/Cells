using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAssertions;

using ParksComputing.Cells.Samples.FileWatcher.Cells;

namespace ParksComputing.Cells.Tests;

public class XmlTransformCellTests {
    [Fact]
    public async Task Strips_Whitespace() {
        var tmp = Path.GetTempFileName();
        await File.WriteAllTextAsync(tmp, "<root>\n  <id>1</id>\n</root>");

        var seed = new FileSeed(tmp, "application/xml");
        var cell = new XmlTransformCell { In = seed };

        await cell.ExecuteAsync();

        cell.Out.Should().Be("<root><id>1</id></root>");
    }
}
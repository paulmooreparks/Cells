using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAssertions;

using ParksComputing.Cells.Samples.FileWatcher;
using ParksComputing.Cells.Samples.FileWatcher.Cells;

namespace ParksComputing.Cells.Tests;

public class CsvTransformTests {
    [Fact]
    public async Task Converts_Csv_To_Json_Array() {
        var tmp = Path.GetTempFileName();
        await File.WriteAllTextAsync(tmp, "id,name\n1,Paul");

        var seed = new FileSeed(tmp, "text/csv");
        var cell = new CsvTransformCell { In = seed };

        await cell.ExecuteAsync();

        cell.Out.Should().Contain("\"id\":\"1\"").And.Contain("\"name\":\"Paul\"");
    }
}
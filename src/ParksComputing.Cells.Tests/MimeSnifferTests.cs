using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAssertions;

using ParksComputing.Cells.Samples.FileWatcher;
using ParksComputing.Cells.Samples.FileWatcher.Cells;

namespace ParksComputing.Cells.Tests;

public class MimeSnifferTests {
    [Theory]
    [InlineData("foo.csv", "text/csv")]
    [InlineData("bar.JSON", "application/json")]
    public async Task Sets_Mime_Based_On_Extension(string fileName, string expected) {
        var seed = new FileSeed(fileName, "");
        var cell = new MimeSnifferCell { In = seed };

        await cell.ExecuteAsync();

        cell.Out.Mime.Should().Be(expected);
    }
}

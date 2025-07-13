using ParksComputing.Cells;
using ParksComputing.Cells.Samples.FileWatcher.Cells;

namespace ParksComputing.Cells.Samples.FileWatcher;

public static class FileWatcherMesh {
    public static Mesh<FileSeed, string> Build(string uploadFolder) {
        var b = MeshBuilder.Create();

        var root = b.Add(new MimeSnifferCell());

        var csv = b.Add(
            Conditional.Create(
                new CsvTransformCell(), 
                s => s.Mime.Equals("text/csv", StringComparison.OrdinalIgnoreCase)
            )
        );

        var json = b.Add(
            Conditional.Create(
                new JsonTransformCell(), 
                s => s.Mime.Equals("application/json", StringComparison.OrdinalIgnoreCase)
            )
        );

        var xml = b.Add(
            Conditional.Create(
                new XmlTransformCell(), 
                s => s.Mime.Equals("application/xml", StringComparison.OrdinalIgnoreCase)
            )
        );

        var xfer = b.Add(
            Conditional.Create(
                new XferTransformCell(), 
                s => s.Mime.Equals("application/xfer", StringComparison.OrdinalIgnoreCase)
            )
        );

        var upload = b.Add(new UploadCell(uploadFolder));

        b.Connect(root, csv);
        b.Connect(root, json);
        b.Connect(root, xml);
        b.Connect(root, xfer);

        var merge = b.Add<ConditionalResult<string>, List<string>>(new CollectMatchesCell());

        b.Connect(csv, merge);
        b.Connect(json, merge);
        b.Connect(xml, merge);
        b.Connect(xfer, merge);

        b.Connect(merge, upload);      // single UploadCell
        return b.Build(root, upload);
    }
}

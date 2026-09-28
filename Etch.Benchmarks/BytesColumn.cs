using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Etch.Benchmarks;

public sealed class BytesColumn : IColumn
{
    private static string PathFor(string type, string method) =>
        Path.Combine(Path.GetTempPath(), $"etch-bytes-{type}-{method}.txt");

    public static void Save(string type, string method, long bytes) =>
        File.WriteAllText(PathFor(type, method), bytes.ToString());

    public static void ClearAll()
    {
        foreach (string file in Directory.GetFiles(Path.GetTempPath(), "etch-bytes-*.txt"))
            File.Delete(file);
    }

    public string Id => nameof(BytesColumn);
    public string ColumnName => "Bytes/frame";
    public string Legend => "Bytes written to the output stream by the last frame";
    public bool AlwaysShow => true;
    public ColumnCategory Category => ColumnCategory.Custom;
    public int PriorityInCategory => 0;
    public bool IsNumeric => true;
    public UnitType UnitType => UnitType.Dimensionless;

    public bool IsAvailable(Summary summary) => true;
    public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase) => false;

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase)
    {
        string path = PathFor(benchmarkCase.Descriptor.Type.Name, benchmarkCase.Descriptor.WorkloadMethod.Name);
        return File.Exists(path) ? File.ReadAllText(path) : "NA";
    }

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style) =>
        GetValue(summary, benchmarkCase);
}
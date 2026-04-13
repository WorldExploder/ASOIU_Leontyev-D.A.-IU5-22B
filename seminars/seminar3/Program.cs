using System.Text;

Console.OutputEncoding = Encoding.UTF8;

var dep = ReadCsv(File.OpenText("dep.csv"), ';');
var dev = ReadCsv(File.OpenText("dev.csv"), ';');

Console.WriteLine("\n=== Таблица dep ===");
PrintTable(dep);

Console.WriteLine("\n=== Таблица dev ===");
PrintTable(dev);

var proj = Projection(dev, "dev_name");
Console.WriteLine("\n=== Projection(dev, dev_name) ===");
PrintTable(proj);

var where = Where(dev, "dep_id", "2");
Console.WriteLine("\n=== Where(dev, dep_id, 2) ===");
PrintTable(where);

var join = Join(dev, dep, "dep_id", "dep_id");
Console.WriteLine("\n=== Join(dev, dep, dep_id, dep_id) ===");
PrintTable(join);

var group = GroupAvg(dev, "dep_id", "dev_commits");
Console.WriteLine("\n=== GroupAvg(dev, dep_id, dev_commits) ===");
PrintTable(group);

static CsvTable ReadCsv(TextReader reader, char separator)
{
    string? headerLine = reader.ReadLine();
    if (headerLine is null)
        throw new InvalidOperationException("CSV пустой: нет строки заголовков.");

    string[] headers = headerLine.Split(separator);
    var rows = new List<CsvRow>();

    string? line;
    while ((line = reader.ReadLine()) is not null)
    {
        if (string.IsNullOrWhiteSpace(line))
            continue;

        string[] parts = line.Split(separator);

        if (parts.Length != headers.Length)
            throw new InvalidOperationException(
                $"Некорректная строка CSV: ожидалось {headers.Length} полей, получено {parts.Length}."
            );

        rows.Add(new CsvRow(parts));
    }

    return new CsvTable(headers, rows);
}

static void WriteCsv(TextWriter writer, CsvTable table, char separator)
{
    writer.WriteLine(string.Join(separator, table.Headers));
    foreach (var row in table.Rows)
        writer.WriteLine(string.Join(separator, row.Fields));
}

static void PrintTable(CsvTable table)
{
    const int width = 20;

    foreach (var header in table.Headers)
        Console.Write($"{header,-width}");
    Console.WriteLine();

    Console.WriteLine(new string('-', width * table.Headers.Length));

    foreach (var row in table.Rows)
    {
        foreach (var field in row.Fields)
            Console.Write($"{field,-width}");
        Console.WriteLine();
    }
}

static int FindColumnIndex(CsvTable table, string columnName)
{
    int index = Array.IndexOf(table.Headers, columnName);

    if (index < 0)
        throw new ArgumentException(
            $"Колонка '{columnName}' не найдена. Доступные колонки: {string.Join(", ", table.Headers)}"
        );

    return index;
}

static CsvTable Projection(CsvTable table, string columnName)
{
    int colIndex = FindColumnIndex(table, columnName);

    string[] newHeaders = { columnName };
    var newRows = new List<CsvRow>();

    foreach (var row in table.Rows)
        newRows.Add(new CsvRow(new[] { row.Fields[colIndex] }));

    return new CsvTable(newHeaders, newRows);
}

static CsvTable Where(CsvTable table, string columnName, string value)
{
    int colIndex = FindColumnIndex(table, columnName);
    var newRows = new List<CsvRow>();

    foreach (var row in table.Rows)
    {
        if (row.Fields[colIndex] == value)
            newRows.Add(row);
    }

    return new CsvTable(table.Headers, newRows);
}

static CsvTable Join(CsvTable left, CsvTable right, string leftKey, string rightKey)
{
    int leftKeyIndex = FindColumnIndex(left, leftKey);
    int rightKeyIndex = FindColumnIndex(right, rightKey);

    var newHeaders = new string[left.Headers.Length + right.Headers.Length];

    for (int i = 0; i < left.Headers.Length; i++)
        newHeaders[i] = left.Headers[i];

    for (int i = 0; i < right.Headers.Length; i++)
        newHeaders[left.Headers.Length + i] = right.Headers[i];

    var newRows = new List<CsvRow>();

    foreach (var leftRow in left.Rows)
    {
        foreach (var rightRow in right.Rows)
        {
            if (leftRow.Fields[leftKeyIndex] == rightRow.Fields[rightKeyIndex])
            {
                var fields = new string[leftRow.Fields.Length + rightRow.Fields.Length];

                for (int i = 0; i < leftRow.Fields.Length; i++)
                    fields[i] = leftRow.Fields[i];

                for (int i = 0; i < rightRow.Fields.Length; i++)
                    fields[leftRow.Fields.Length + i] = rightRow.Fields[i];

                newRows.Add(new CsvRow(fields));
            }
        }
    }

    return new CsvTable(newHeaders, newRows);
}

static double Average(List<double> values)
{
    double sum = 0;
    foreach (double value in values)
        sum += value;
    return sum / values.Count;
}

static CsvTable GroupAvg(CsvTable table, string groupColumn, string valueColumn)
{
    int groupIndex = FindColumnIndex(table, groupColumn);
    int valueIndex = FindColumnIndex(table, valueColumn);

    var groups = new Dictionary<string, List<double>>();

    foreach (var row in table.Rows)
    {
        string key = row.Fields[groupIndex];
        double value = double.Parse(row.Fields[valueIndex]);

        if (!groups.ContainsKey(key))
            groups[key] = new List<double>();

        groups[key].Add(value);
    }

    string[] newHeaders = { groupColumn, "avg_" + valueColumn };
    var newRows = new List<CsvRow>();

    foreach (var pair in groups)
        newRows.Add(new CsvRow(new[] { pair.Key, Average(pair.Value).ToString("F2") }));

    return new CsvTable(newHeaders, newRows);
}

record CsvRow(string[] Fields);
record CsvTable(string[] Headers, List<CsvRow> Rows);
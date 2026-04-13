using System.Text;
using Microsoft.Data.Sqlite;

Console.OutputEncoding = Encoding.UTF8;

var db = new DatabaseManager("university.db");
db.InitializeDatabase("chairs.csv", "teachers.csv");

while (true)
{
    Console.WriteLine("===== МЕНЮ =====");
    Console.WriteLine("1. Показать кафедры");
    Console.WriteLine("2. Показать преподавателей");
    Console.WriteLine("3. Добавить преподавателя");
    Console.WriteLine("4. Изменить преподавателя");
    Console.WriteLine("5. Удалить преподавателя");
    Console.WriteLine("6. Отчёты");
    Console.WriteLine("0. Выход");
    Console.Write("Выберите пункт: ");

    string? choice = Console.ReadLine();
    Console.WriteLine();

    switch (choice)
    {
        case "1":
            ShowChairs(db);
            break;
        case "2":
            ShowTeachers(db);
            break;
        case "3":
            AddTeacher(db);
            break;
        case "4":
            EditTeacher(db);
            break;
        case "5":
            DeleteTeacher(db);
            break;
        case "6":
            ShowReports(db);
            break;
        case "0":
            return;
        default:
            Console.WriteLine("Неверный пункт меню.");
            break;
    }

    Console.WriteLine();
}

static void ShowChairs(DatabaseManager db)
{
    Console.WriteLine("Список кафедр:");
    foreach (var chair in db.GetAllChairs())
        Console.WriteLine(chair);
}

static void ShowTeachers(DatabaseManager db)
{
    Console.WriteLine("Список преподавателей:");
    foreach (var teacher in db.GetAllTeachers())
        Console.WriteLine(teacher);
}

static void AddTeacher(DatabaseManager db)
{
    try
    {
        Console.WriteLine("Доступные кафедры:");
        foreach (var chair in db.GetAllChairs())
            Console.WriteLine(chair);

        Console.Write("Введите id кафедры: ");
        if (!int.TryParse(Console.ReadLine(), out int chairId))
        {
            Console.WriteLine("Ошибка: id кафедры должен быть целым числом.");
            return;
        }

        Console.Write("Введите ФИО преподавателя: ");
        string name = Console.ReadLine() ?? "";

        Console.Write("Введите количество публикаций: ");
        if (!int.TryParse(Console.ReadLine(), out int publications))
        {
            Console.WriteLine("Ошибка: количество публикаций должно быть целым числом.");
            return;
        }

        var teacher = new Teacher(0, chairId, name, publications);
        db.AddTeacher(teacher);

        Console.WriteLine("Преподаватель добавлен.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
    }
}

static void EditTeacher(DatabaseManager db)
{
    try
    {
        Console.Write("Введите id преподавателя для изменения: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Ошибка: id должен быть целым числом.");
            return;
        }

        var teacher = db.GetTeacherById(id);
        if (teacher == null)
        {
            Console.WriteLine("Преподаватель не найден.");
            return;
        }

        Console.WriteLine("Текущие данные:");
        Console.WriteLine(teacher);

        Console.WriteLine("Доступные кафедры:");
        foreach (var chair in db.GetAllChairs())
            Console.WriteLine(chair);

        Console.Write($"Новый id кафедры [{teacher.ChairId}]: ");
        string? chairInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(chairInput))
        {
            if (int.TryParse(chairInput, out int newChairId))
                teacher.ChairId = newChairId;
            else
            {
                Console.WriteLine("Ошибка: id кафедры должен быть целым числом.");
                return;
            }
        }

        Console.Write($"Новое ФИО [{teacher.Name}]: ");
        string? nameInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(nameInput))
            teacher.Name = nameInput;

        Console.Write($"Новое количество публикаций [{teacher.Publications}]: ");
        string? pubInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(pubInput))
        {
            if (int.TryParse(pubInput, out int newPublications))
                teacher.Publications = newPublications;
            else
            {
                Console.WriteLine("Ошибка: количество публикаций должно быть целым числом.");
                return;
            }
        }

        db.UpdateTeacher(teacher);
        Console.WriteLine("Данные преподавателя обновлены.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
    }
}

static void DeleteTeacher(DatabaseManager db)
{
    try
    {
        Console.Write("Введите id преподавателя для удаления: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Ошибка: id должен быть целым числом.");
            return;
        }

        db.DeleteTeacher(id);
        Console.WriteLine("Преподаватель удалён.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
    }
}

static void ShowReports(DatabaseManager db)
{
    while (true)
    {
        Console.WriteLine("===== ОТЧЁТЫ =====");
        Console.WriteLine("1. Полный список преподавателей с кафедрами");
        Console.WriteLine("2. Количество преподавателей по кафедрам");
        Console.WriteLine("3. Среднее число публикаций по кафедрам");
        Console.WriteLine("4. Сохранить отчёт 1 в файл");
        Console.WriteLine("5. Сохранить отчёт 2 в файл");
        Console.WriteLine("6. Сохранить отчёт 3 в файл");
        Console.WriteLine("0. Назад");
        Console.Write("Выберите пункт: ");

        string? choice = Console.ReadLine();
        Console.WriteLine();

        switch (choice)
        {
            case "1":
                BuildReport1(db).Print();
                break;

            case "2":
                BuildReport2(db).Print();
                break;

            case "3":
                BuildReport3(db).Print();
                break;

            case "4":
                BuildReport1(db).SaveToFile(GetProjectFilePath("report1.txt"));
                Console.WriteLine("Отчёт сохранён в папку проекта: report1.txt");
                break;

            case "5":
                BuildReport2(db).SaveToFile(GetProjectFilePath("report2.txt"));
                Console.WriteLine("Отчёт сохранён в папку проекта: report2.txt");
                break;

            case "6":
                BuildReport3(db).SaveToFile(GetProjectFilePath("report3.txt"));
                Console.WriteLine("Отчёт сохранён в папку проекта: report3.txt");
                break;

            case "0":
                return;

            default:
                Console.WriteLine("Неверный пункт меню.");
                break;
        }

        Console.WriteLine();
    }
}

static ReportBuilder BuildReport1(DatabaseManager db)
{
    return new ReportBuilder(db)
        .Query(@"
SELECT t.teacher_name, c.chair_name, t.publications
FROM teacher t
JOIN chair c ON t.chair_id = c.chair_id
ORDER BY t.teacher_name")
        .Title("Преподаватели по кафедрам")
        .Header("Преподаватель", "Кафедра", "Публикации")
        .ColumnWidths(25, 25, 15);
}

static ReportBuilder BuildReport2(DatabaseManager db)
{
    return new ReportBuilder(db)
        .Query(@"
SELECT c.chair_name, COUNT(*) AS teacher_count
FROM teacher t
JOIN chair c ON t.chair_id = c.chair_id
GROUP BY c.chair_name
ORDER BY c.chair_name")
        .Title("Количество преподавателей по кафедрам")
        .Header("Кафедра", "Количество")
        .ColumnWidths(30, 15);
}

static ReportBuilder BuildReport3(DatabaseManager db)
{
    return new ReportBuilder(db)
        .Query(@"
SELECT c.chair_name, ROUND(AVG(t.publications), 2) AS avg_publications
FROM teacher t
JOIN chair c ON t.chair_id = c.chair_id
GROUP BY c.chair_name
ORDER BY avg_publications DESC")
        .Title("Среднее количество публикаций по кафедрам")
        .Header("Кафедра", "Среднее")
        .ColumnWidths(30, 15);
}

static string GetProjectFilePath(string fileName)
{
    string currentDir = AppContext.BaseDirectory;
    DirectoryInfo? dir = new DirectoryInfo(currentDir);

    while (dir != null && dir.Name != "bin")
    {
        dir = dir.Parent;
    }

    if (dir?.Parent != null)
        return Path.Combine(dir.Parent.FullName, fileName);

    return Path.Combine(Directory.GetCurrentDirectory(), fileName);
}

/// <summary>
/// Кафедра
/// </summary>
class Chair
{
    public int Id { get; set; }
    public string Name { get; set; }

    public Chair(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public Chair() : this(0, "") { }

    public override string ToString() => $"[{Id}] {Name}";
}

/// <summary>
/// Преподаватель
/// </summary>
class Teacher
{
    public int Id { get; set; }
    public int ChairId { get; set; }
    public string Name { get; set; }

    private int _publications;

    public int Publications
    {
        get => _publications;
        set
        {
            if (value < 0)
                throw new ArgumentException("Количество публикаций не может быть отрицательным");
            _publications = value;
        }
    }

    public Teacher(int id, int chairId, string name, int publications)
    {
        Id = id;
        ChairId = chairId;
        Name = name;
        Publications = publications;
    }

    public Teacher() : this(0, 0, "", 0) { }

    public override string ToString()
        => $"[{Id}] {Name}, кафедра #{ChairId}, публикаций: {Publications}";
}

/// <summary>
/// Управление SQLite
/// </summary>
class DatabaseManager
{
    private readonly string _connectionString;

    public DatabaseManager(string dbPath)
    {
        _connectionString = $"Data Source={dbPath}";
    }

    public void InitializeDatabase(string chairCsvPath, string teacherCsvPath)
    {
        CreateTables();

        if (GetAllChairs().Count == 0 && File.Exists(chairCsvPath))
        {
            ImportChairsFromCsv(chairCsvPath);
            Console.WriteLine($"[OK] Загружены кафедры из {chairCsvPath}");
        }

        if (GetAllTeachers().Count == 0 && File.Exists(teacherCsvPath))
        {
            ImportTeachersFromCsv(teacherCsvPath);
            Console.WriteLine($"[OK] Загружены преподаватели из {teacherCsvPath}");
        }
    }

    private void CreateTables()
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS chair (
    chair_id INTEGER PRIMARY KEY AUTOINCREMENT,
    chair_name TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS teacher (
    teacher_id INTEGER PRIMARY KEY AUTOINCREMENT,
    chair_id INTEGER NOT NULL,
    teacher_name TEXT NOT NULL,
    publications INTEGER NOT NULL,
    FOREIGN KEY (chair_id) REFERENCES chair(chair_id)
);";
        cmd.ExecuteNonQuery();
    }

    private void ImportChairsFromCsv(string path)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        string[] lines = File.ReadAllLines(path);

        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(';');
            if (parts.Length < 2)
                continue;

            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO chair (chair_id, chair_name) VALUES (@id, @name)";
            cmd.Parameters.AddWithValue("@id", int.Parse(parts[0]));
            cmd.Parameters.AddWithValue("@name", parts[1]);
            cmd.ExecuteNonQuery();
        }
    }

    private void ImportTeachersFromCsv(string path)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        string[] lines = File.ReadAllLines(path);

        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(';');
            if (parts.Length < 4)
                continue;

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
INSERT INTO teacher (teacher_id, chair_id, teacher_name, publications)
VALUES (@id, @chairId, @name, @publications)";
            cmd.Parameters.AddWithValue("@id", int.Parse(parts[0]));
            cmd.Parameters.AddWithValue("@chairId", int.Parse(parts[1]));
            cmd.Parameters.AddWithValue("@name", parts[2]);
            cmd.Parameters.AddWithValue("@publications", int.Parse(parts[3]));
            cmd.ExecuteNonQuery();
        }
    }

    public List<Chair> GetAllChairs()
    {
        var result = new List<Chair>();

        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT chair_id, chair_name FROM chair ORDER BY chair_id";

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new Chair(
                reader.GetInt32(0),
                reader.GetString(1)
            ));
        }

        return result;
    }

    public List<Teacher> GetAllTeachers()
    {
        var result = new List<Teacher>();

        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT teacher_id, chair_id, teacher_name, publications FROM teacher ORDER BY teacher_id";

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new Teacher(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetString(2),
                reader.GetInt32(3)
            ));
        }

        return result;
    }

    public Teacher? GetTeacherById(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
SELECT teacher_id, chair_id, teacher_name, publications
FROM teacher
WHERE teacher_id = @id";
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Teacher(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetString(2),
                reader.GetInt32(3)
            );
        }

        return null;
    }

    public void AddTeacher(Teacher teacher)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
INSERT INTO teacher (chair_id, teacher_name, publications)
VALUES (@chairId, @name, @publications)";
        cmd.Parameters.AddWithValue("@chairId", teacher.ChairId);
        cmd.Parameters.AddWithValue("@name", teacher.Name);
        cmd.Parameters.AddWithValue("@publications", teacher.Publications);
        cmd.ExecuteNonQuery();
    }

    public void UpdateTeacher(Teacher teacher)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
UPDATE teacher
SET chair_id = @chairId,
    teacher_name = @name,
    publications = @publications
WHERE teacher_id = @id";
        cmd.Parameters.AddWithValue("@id", teacher.Id);
        cmd.Parameters.AddWithValue("@chairId", teacher.ChairId);
        cmd.Parameters.AddWithValue("@name", teacher.Name);
        cmd.Parameters.AddWithValue("@publications", teacher.Publications);
        cmd.ExecuteNonQuery();
    }

    public void DeleteTeacher(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM teacher WHERE teacher_id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public (string[] columns, List<string[]> rows) ExecuteQuery(string sql)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = sql;

        using var reader = cmd.ExecuteReader();

        string[] columns = new string[reader.FieldCount];
        for (int i = 0; i < reader.FieldCount; i++)
            columns[i] = reader.GetName(i);

        var rows = new List<string[]>();

        while (reader.Read())
        {
            string[] row = new string[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
                row[i] = reader.GetValue(i)?.ToString() ?? "";
            rows.Add(row);
        }

        return (columns, rows);
    }
}

/// <summary>
/// Построитель отчётов
/// </summary>
class ReportBuilder
{
    private readonly DatabaseManager _db;
    private string _sql = "";
    private string _title = "";
    private string[] _headers = Array.Empty<string>();
    private int[] _widths = Array.Empty<int>();

    public ReportBuilder(DatabaseManager db)
    {
        _db = db;
    }

    public ReportBuilder Query(string sql)
    {
        _sql = sql;
        return this;
    }

    public ReportBuilder Title(string text)
    {
        _title = text;
        return this;
    }

    public ReportBuilder Header(params string[] columns)
    {
        _headers = columns;
        return this;
    }

    public ReportBuilder ColumnWidths(params int[] widths)
    {
        _widths = widths;
        return this;
    }

    public string Build()
    {
        var (_, rows) = _db.ExecuteQuery(_sql);
        var sb = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(_title))
            sb.AppendLine($"=== {_title} ===");

        for (int i = 0; i < _headers.Length; i++)
        {
            int width = i < _widths.Length ? _widths[i] : 15;
            sb.Append(_headers[i].PadRight(width));
        }

        sb.AppendLine();

        int totalWidth = 0;
        for (int i = 0; i < _headers.Length; i++)
            totalWidth += i < _widths.Length ? _widths[i] : 15;

        sb.AppendLine(new string('-', totalWidth));

        foreach (var row in rows)
        {
            for (int i = 0; i < row.Length; i++)
            {
                int width = i < _widths.Length ? _widths[i] : 15;
                sb.Append(row[i].PadRight(width));
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

    public ReportBuilder Print()
    {
        Console.WriteLine(Build());
        return this;
    }

    public ReportBuilder SaveToFile(string path)
    {
        File.WriteAllText(path, Build(), Encoding.UTF8);
        return this;
    }
}
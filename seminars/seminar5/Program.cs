using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

interface ILogger
{
    void Log(string message);
}

class NaiveConsoleLogger
{
    public void Log(string message)
    {
        Console.WriteLine($"[LOG] {message}");
    }
}

class ConsoleLogger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine($"[LOG] {message}");
    }
}

class NaiveFileLogger
{
    private string _filePath;

    public NaiveFileLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Log(string message)
    {
        File.AppendAllText(_filePath, $"[LOG] {message}{Environment.NewLine}");
    }
}

class FileLogger : ILogger
{
    private string _filePath;

    public FileLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Log(string message)
    {
        File.AppendAllText(_filePath, $"[LOG] {message}{Environment.NewLine}");
    }
}

class NullLogger : ILogger
{
    public void Log(string message) { }
}

class NaiveBookCatalogService
{
    private NaiveConsoleLogger _logger = new NaiveConsoleLogger();

    public void AddBook(string title, string author)
    {
        _logger.Log($"Добавлена книга: «{title}» — {author}");
    }

    public void RemoveBook(string title)
    {
        _logger.Log($"Удалена книга: «{title}»");
    }
}

class NaiveBookCatalogServiceLogFlag
{
    private bool _useFile;

    public NaiveBookCatalogServiceLogFlag(bool useFile = false)
    {
        _useFile = useFile;
    }

    public void AddBook(string title, string author)
    {
        if (_useFile)
            File.AppendAllText("log.txt", $"Добавлена книга: «{title}»{Environment.NewLine}");
        else
            Console.WriteLine($"[LOG] Добавлена книга: «{title}»");
    }
}

class BookCatalogService_DI_Constructor
{
    private ILogger _logger;

    public BookCatalogService_DI_Constructor(ILogger logger)
    {
        _logger = logger;
    }

    public void AddBook(string title, string author)
    {
        _logger.Log($"Добавлена книга: «{title}» — {author}");
    }

    public void RemoveBook(string title)
    {
        _logger.Log($"Удалена книга: «{title}»");
    }
}

class BookCatalogService_DI_Property
{
    public ILogger Logger { get; set; } = new NullLogger();

    public void AddBook(string title, string author)
    {
        Logger.Log($"Добавлена книга: «{title}» — {author}");
    }

    public void RemoveBook(string title)
    {
        Logger.Log($"Удалена книга: «{title}»");
    }
}

class BookCatalogService_DI_Method
{
    public void AddBook(string title, string author, ILogger logger)
    {
        logger.Log($"Добавлена книга: «{title}» — {author}");
    }
}

interface IBookStorage
{
    void Save(string title, string author);
}

class InMemoryBookStorage : IBookStorage
{
    private ILogger _logger;
    private List<string> _books = new();

    public InMemoryBookStorage(ILogger logger)
    {
        _logger = logger;
    }

    public void Save(string title, string author)
    {
        _books.Add($"«{title}» — {author}");
        _logger.Log($" [STORAGE] Сохранено: «{title}»");
    }
}

class BookCatalogService
{
    private ILogger _logger;
    private IBookStorage _storage;

    public BookCatalogService(ILogger logger, IBookStorage storage)
    {
        _logger = logger;
        _storage = storage;
    }

    public void AddBook(string title, string author)
    {
        _storage.Save(title, author);
        _logger.Log($"Добавлена книга: «{title}» — {author}");
    }

    public void RemoveBook(string title)
    {
        _logger.Log($"Удалена книга: «{title}»");
    }
}

static class AppServices
{
    public static ServiceProvider Provider { get; set; }
    public static T Get<T>() => Provider.GetRequiredService<T>();
}

static class CurrentLogger
{
    public static ILogger Instance { get; set; } = new NullLogger();
}

class FixedBookCatalogService
{
    private readonly ILogger _logger;
    private readonly IBookStorage _storage;

    public FixedBookCatalogService(ILogger logger, IBookStorage storage)
    {
        _logger = logger;
        _storage = storage;
    }

    public void AddBook(string title, string author)
    {
        _storage.Save(title, author);
        _logger.Log($"Добавлена книга: «{title}» — {author}");
    }

    public void RemoveBook(string title)
    {
        _logger.Log($"Удалена книга: «{title}»");
    }
}

class Program
{
    static void Main()
    {
        if (File.Exists("log.txt")) File.Delete("log.txt");
        if (File.Exists("audit.log")) File.Delete("audit.log");
        if (File.Exists("catalog.log")) File.Delete("catalog.log");

        Console.WriteLine("=== Наивная реализация ===");
        NaiveBookCatalogService service = new();
        service.AddBook("Евгений Онегин", "Пушкин");
        service.RemoveBook("Евгений Онегин");

        Console.WriteLine();

        Console.WriteLine("=== Внедрение через конструктор ===");
        var s1 = new BookCatalogService_DI_Constructor(new ConsoleLogger());
        s1.AddBook("Евгений Онегин", "Пушкин");

        var s2 = new BookCatalogService_DI_Constructor(new FileLogger("log.txt"));
        s2.AddBook("Сборник стихотворений", "Пушкин");

        Console.WriteLine();

        Console.WriteLine("=== Внедрение через свойство ===");
        var s3 = new BookCatalogService_DI_Property();
        s3.AddBook("Евгений Онегин", "Пушкин");
        s3.Logger = new ConsoleLogger();
        s3.AddBook("Сборник стихотворений", "Пушкин");

        Console.WriteLine();

        Console.WriteLine("=== Внедрение через параметр метода ===");
        var s4 = new BookCatalogService_DI_Method();
        s4.AddBook("Евгений Онегин", "Пушкин", new ConsoleLogger());
        s4.AddBook("Сборник стихотворений", "Пушкин", new FileLogger("audit.log"));

        Console.WriteLine();

        Console.WriteLine("=== Точка сборки ===");
        ILogger logger = new ConsoleLogger();
        IBookStorage storage = new InMemoryBookStorage(logger);
        var s5 = new BookCatalogService(logger, storage);
        s5.AddBook("Евгений Онегин", "Пушкин");
        s5.AddBook("Сборник стихотворений", "Пушкин");

        Console.WriteLine();

        Console.WriteLine("=== Scoped ===");
        var scopedServices = new ServiceCollection();
        scopedServices.AddSingleton<ILogger, ConsoleLogger>();
        scopedServices.AddScoped<IBookStorage, InMemoryBookStorage>();

        var scopedProvider = scopedServices.BuildServiceProvider(
            new ServiceProviderOptions { ValidateOnBuild = true }
        );

        using (var scope1 = scopedProvider.CreateScope())
        {
            var st1 = scope1.ServiceProvider.GetRequiredService<IBookStorage>();
            var st2 = scope1.ServiceProvider.GetRequiredService<IBookStorage>();
            Console.WriteLine(object.ReferenceEquals(st1, st2));
        }

        using (var scope2 = scopedProvider.CreateScope())
        {
            var st3 = scope2.ServiceProvider.GetRequiredService<IBookStorage>();
        }

        Console.WriteLine();

        Console.WriteLine("=== Точка сборки с фреймворком ===");
        var services = new ServiceCollection();
        services.AddSingleton<ILogger, ConsoleLogger>();
        services.AddSingleton<IBookStorage, InMemoryBookStorage>();
        services.AddTransient<BookCatalogService>();

        var provider = services.BuildServiceProvider(
            new ServiceProviderOptions { ValidateOnBuild = true }
        );

        var s7 = provider.GetRequiredService<BookCatalogService>();
        s7.AddBook("Евгений Онегин", "Пушкин");

        Console.WriteLine();

        Console.WriteLine("=== Исправленное контрольное задание ===");
        ILogger fixedLogger = new ConsoleLogger();
        IBookStorage fixedStorage = new InMemoryBookStorage(fixedLogger);
        var fixedService = new FixedBookCatalogService(fixedLogger, fixedStorage);

        fixedService.AddBook("Мёртвые души", "Гоголь");
        fixedService.RemoveBook("Мёртвые души");

        Console.WriteLine();

        Console.WriteLine("=== Содержимое log.txt ===");
        Console.WriteLine(File.Exists("log.txt") ? File.ReadAllText("log.txt") : "(файл пуст или не создан)");

        Console.WriteLine("=== Содержимое audit.log ===");
        Console.WriteLine(File.Exists("audit.log") ? File.ReadAllText("audit.log") : "(файл пуст или не создан)");
    }
}
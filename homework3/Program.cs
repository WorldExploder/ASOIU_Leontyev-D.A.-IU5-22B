using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

class Program
{
    static void Main()
    {
        DatabaseInitializer.Initialize();
        
        while (true)
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine("   Управление кафедрами и преподавателями");
            Console.WriteLine("========================================");
            Console.WriteLine("1. Работа с кафедрами");
            Console.WriteLine("2. Работа с преподавателями");
            Console.WriteLine("3. Отчёты");
            Console.WriteLine("0. Выход");
            Console.Write("\nВыберите пункт: ");
            
            var choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1": ManageDepartments(); break;
                case "2": ManageTeachers(); break;
                case "3": ShowReports(); break;
                case "0": return;
            }
        }
    }
    
    static void ManageDepartments()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("========== Управление кафедрами ==========");
            Console.WriteLine("1. Список кафедр");
            Console.WriteLine("2. Добавить кафедру");
            Console.WriteLine("3. Редактировать кафедру");
            Console.WriteLine("4. Удалить кафедру");
            Console.WriteLine("0. Назад");
            Console.Write("\nВыберите пункт: ");
            
            var choice = Console.ReadLine();
            
            using var context = new AppDbContext();
            
            switch (choice)
            {
                case "1":
                    var depts = context.Departments.OrderBy(d => d.Name).ToList();
                    Console.WriteLine("\n=== Список кафедр ===");
                    foreach (var d in depts)
                        Console.WriteLine($"{d.Id}. {d.Name}");
                    break;
                    
                case "2":
                    Console.Write("\nНазвание кафедры: ");
                    var name = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        context.Departments.Add(new Department { Name = name });
                        context.SaveChanges();
                        Console.WriteLine("✓ Кафедра добавлена!");
                    }
                    else
                        Console.WriteLine("✗ Ошибка: название не может быть пустым!");
                    break;
                    
                case "3":
                    Console.Write("\nID кафедры: ");
                    if (int.TryParse(Console.ReadLine(), out int id))
                    {
                        var dept = context.Departments.Find(id);
                        if (dept != null)
                        {
                            Console.Write($"Новое название (было: {dept.Name}): ");
                            var newName = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(newName))
                            {
                                dept.Name = newName;
                                context.SaveChanges();
                                Console.WriteLine("✓ Кафедра изменена!");
                            }
                        }
                        else
                            Console.WriteLine("✗ Кафедра не найдена!");
                    }
                    break;
                    
                case "4":
                    Console.Write("\nID кафедры: ");
                    if (int.TryParse(Console.ReadLine(), out int deleteId))
                    {
                        var dept = context.Departments.Include(d => d.Teachers).FirstOrDefault(d => d.Id == deleteId);
                        if (dept != null)
                        {
                            if (dept.Teachers.Any())
                                Console.WriteLine("✗ Нельзя удалить! На кафедре есть преподаватели.");
                            else
                            {
                                context.Departments.Remove(dept);
                                context.SaveChanges();
                                Console.WriteLine("✓ Кафедра удалена!");
                            }
                        }
                        else
                            Console.WriteLine("✗ Кафедра не найдена!");
                    }
                    break;
                    
                case "0":
                    return;
            }
            
            Console.WriteLine("\nНажмите Enter...");
            Console.ReadLine();
        }
    }
    
    static void ManageTeachers()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("========== Управление преподавателями ==========");
            Console.WriteLine("1. Список преподавателей");
            Console.WriteLine("2. Добавить преподавателя");
            Console.WriteLine("3. Редактировать преподавателя");
            Console.WriteLine("4. Удалить преподавателя");
            Console.WriteLine("0. Назад");
            Console.Write("\nВыберите пункт: ");
            
            var choice = Console.ReadLine();
            
            using var context = new AppDbContext();
            
            switch (choice)
            {
                case "1":
                    var teachers = context.Teachers
                        .Include(t => t.Department)
                        .OrderBy(t => t.Name)
                        .ToList();
                    Console.WriteLine("\n=== Список преподавателей ===");
                    foreach (var t in teachers)
                        Console.WriteLine($"{t.Id}. {t.Name} - {t.Publications} публикаций, кафедра: {t.Department?.Name ?? "Нет"}");
                    break;
                    
                case "2":
                    Console.Write("\nИмя преподавателя: ");
                    var name = Console.ReadLine();
                    Console.Write("Количество публикаций: ");
                    if (int.TryParse(Console.ReadLine(), out int pubs))
                    {
                        if (pubs < 0)
                        {
                            Console.WriteLine("✗ Ошибка: публикации не могут быть отрицательными!");
                        }
                        else
                        {
                            Console.WriteLine("\nДоступные кафедры:");
                            var depts = context.Departments.ToList();
                            foreach (var d in depts)
                                Console.WriteLine($"{d.Id}. {d.Name}");
                            Console.Write("Выберите ID кафедры: ");
                            if (int.TryParse(Console.ReadLine(), out int deptId) && context.Departments.Any(d => d.Id == deptId))
                            {
                                context.Teachers.Add(new Teacher 
                                { 
                                    Name = name ?? "", 
                                    Publications = pubs, 
                                    DepartmentId = deptId 
                                });
                                context.SaveChanges();
                                Console.WriteLine("✓ Преподаватель добавлен!");
                            }
                            else
                                Console.WriteLine("✗ Ошибка: кафедра не найдена!");
                        }
                    }
                    break;
                    
                case "3":
                    Console.Write("\nID преподавателя: ");
                    if (int.TryParse(Console.ReadLine(), out int id))
                    {
                        var teacher = context.Teachers.Find(id);
                        if (teacher != null)
                        {
                            Console.Write($"Новое имя (было: {teacher.Name}): ");
                            var newName = Console.ReadLine();
                            Console.Write($"Новое количество публикаций (было: {teacher.Publications}): ");
                            if (int.TryParse(Console.ReadLine(), out int newPubs) && newPubs >= 0)
                            {
                                if (!string.IsNullOrWhiteSpace(newName))
                                    teacher.Name = newName;
                                teacher.Publications = newPubs;
                                context.SaveChanges();
                                Console.WriteLine("✓ Преподаватель изменён!");
                            }
                            else
                                Console.WriteLine("✗ Ошибка: публикации не могут быть отрицательными!");
                        }
                        else
                            Console.WriteLine("✗ Преподаватель не найден!");
                    }
                    break;
                    
                case "4":
                    Console.Write("\nID преподавателя: ");
                    if (int.TryParse(Console.ReadLine(), out int deleteId))
                    {
                        var teacher = context.Teachers.Find(deleteId);
                        if (teacher != null)
                        {
                            context.Teachers.Remove(teacher);
                            context.SaveChanges();
                            Console.WriteLine("✓ Преподаватель удалён!");
                        }
                        else
                            Console.WriteLine("✗ Преподаватель не найден!");
                    }
                    break;
                    
                case "0":
                    return;
            }
            
            Console.WriteLine("\nНажмите Enter...");
            Console.ReadLine();
        }
    }
    
    static void ShowReports()
    {
        Console.Clear();
        using var context = new AppDbContext();
        
        Console.WriteLine("==================== ОТЧЁТЫ ====================\n");
        
        Console.WriteLine("1. ПОЛНЫЙ СПИСОК ПРЕПОДАВАТЕЛЕЙ:");
        Console.WriteLine("----------------------------------------");
        var report1 = context.Teachers
            .Include(t => t.Department)
            .OrderBy(t => t.Name)
            .ToList();
        foreach (var t in report1)
            Console.WriteLine($"   {t.Name,-25} | {t.Publications,3} публикаций | {t.Department?.Name ?? "Нет кафедры"}");
        
        Console.WriteLine("\n2. КОЛИЧЕСТВО ПРЕПОДАВАТЕЛЕЙ ПО КАФЕДРАМ:");
        Console.WriteLine("----------------------------------------");
        var report2 = context.Teachers
            .GroupBy(t => t.Department!.Name)
            .Select(g => new { Department = g.Key, Count = g.Count() })
            .OrderBy(r => r.Department)
            .ToList();
        foreach (var r in report2)
            Console.WriteLine($"   {r.Department,-25} | {r.Count,2} преподавателей");
        
        Console.WriteLine("\n3. СРЕДНЕЕ КОЛИЧЕСТВО ПУБЛИКАЦИЙ ПО КАФЕДРАМ:");
        Console.WriteLine("----------------------------------------");
        var report3 = context.Teachers
            .GroupBy(t => t.Department!.Name)
            .Select(g => new { Department = g.Key, Avg = g.Average(t => t.Publications) })
            .OrderByDescending(r => r.Avg)
            .ToList();
        foreach (var r in report3)
            Console.WriteLine($"   {r.Department,-25} | {Math.Round(r.Avg, 2),6} публикаций (среднее)");
        
        Console.WriteLine("\n================================================");
        Console.WriteLine("\nНажмите Enter...");
        Console.ReadLine();
    }
}
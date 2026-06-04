using System.Linq;

public static class DatabaseInitializer
{
    public static void Initialize()
    {
        using var context = new AppDbContext();
        context.Database.EnsureCreated();

        if (!context.Departments.Any())
        {
            var departments = new[]
            {
                new Department { Name = "ИУ5" },
                new Department { Name = "ИУ6" },
                new Department { Name = "ИУ7" },
                new Department { Name = "ИУ8" }
            };
            context.Departments.AddRange(departments);
            context.SaveChanges();

            var teachers = new[]
            {
                new Teacher { Name = "Иванов И.И.", Publications = 45, DepartmentId = 1 },
                new Teacher { Name = "Петров П.П.", Publications = 30, DepartmentId = 1 },
                new Teacher { Name = "Сидоров С.С.", Publications = 60, DepartmentId = 2 },
                new Teacher { Name = "Кузнецова А.А.", Publications = 25, DepartmentId = 2 },
                new Teacher { Name = "Смирнов В.В.", Publications = 80, DepartmentId = 2 },
                new Teacher { Name = "Васильева Е.Е.", Publications = 15, DepartmentId = 3 },
                new Teacher { Name = "Михайлов Д.Д.", Publications = 50, DepartmentId = 3 },
                new Teacher { Name = "Новикова О.О.", Publications = 90, DepartmentId = 4 },
                new Teacher { Name = "Фёдоров К.К.", Publications = 20, DepartmentId = 4 },
                new Teacher { Name = "Морозов А.А.", Publications = 110, DepartmentId = 4 },
                new Teacher { Name = "Волков И.И.", Publications = 5, DepartmentId = 1 },
                new Teacher { Name = "Алексеев Н.Н.", Publications = 35, DepartmentId = 3 }
            };
            context.Teachers.AddRange(teachers);
            context.SaveChanges();
        }
    }
}
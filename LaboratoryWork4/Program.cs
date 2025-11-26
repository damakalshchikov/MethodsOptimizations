using LaboratoryWork4.Tasks.Task1;
using LaboratoryWork4.Tasks.Task2;

Console.WriteLine("═══════════════════════════════════════════════════════════════");
Console.WriteLine("     ЛАБОРАТОРНАЯ РАБОТА №4 - МЕТОДЫ УСЛОВНОЙ ОПТИМИЗАЦИИ     ");
Console.WriteLine("═══════════════════════════════════════════════════════════════");
Console.WriteLine();

// Меню выбора метода
Console.WriteLine("Выберите метод для запуска:");
Console.WriteLine();
Console.WriteLine("  1. Метод штрафов");
Console.WriteLine("  2. Метод барьерных функций");
Console.WriteLine();
Console.Write("Ваш выбор (1-2): ");

string? choice = Console.ReadLine();
Console.WriteLine();

switch (choice)
{
    case "1":
        RunTask1();
        break;

    case "2":
        RunTask2();
        break;

    default:
        Console.WriteLine("Неверный выбор. Завершение программы.");
        break;
}

static void RunTask1()
{
    var solver = new Task1Solver();
    var result = solver.Solve();
    solver.PrintResult(result);
}

static void RunTask2()
{
    var solver = new Task2Solver();
    var result = solver.Solve();
    solver.PrintResult(result);
}

using LaboratoryWork1.Tasks.Task1;
using LaboratoryWork1.Tasks.Task2;

namespace LaboratoryWork1;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Выберите задание:");
        Console.WriteLine("1 - Задание №1 (Метод половинного деления)");
        Console.WriteLine("2 - Задание №2 (Метод золотого сечения)");
        Console.Write("Ваш выбор: ");

        var choice = Console.ReadLine();
        Console.WriteLine();

        switch (choice)
        {
            case "1":
                var solver1 = new Task1Solver();
                var result1 = solver1.Solve();
                solver1.PrintResult(result1);
                break;

            case "2":
                var solver2 = new Task2Solver();
                var result2 = solver2.Solve();
                solver2.PrintResult(result2);
                break;

            default:
                Console.WriteLine("Неверный выбор. Попробуйте снова.");
                break;
        }
    }
}

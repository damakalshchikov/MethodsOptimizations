using LaboratoryWork1.Core;
using LaboratoryWork1.Models;

namespace LaboratoryWork1.Tasks.Task2;

// Решатель для задания №2
public class Task2Solver
{
    private readonly IFunction _function;
    private readonly IOptimizationMethod _method;

    public Task2Solver()
    {
        _function = new Task2Function();
        _method = new GoldenSectionMethod();
    }

    public OptimizationResult Solve()
    {
        // Параметры из задания
        double a = -1;
        double b = 3;
        double epsilon = 0.3;

        return _method.FindMinimum(_function, a, b, epsilon);
    }

    public void PrintResult(OptimizationResult result)
    {
        Console.WriteLine("=== Задание №2 ===");
        Console.WriteLine(_function.Name);
        Console.WriteLine($"Интервал: [-1; 3]");
        Console.WriteLine($"Точность: ε = 0.3");
        Console.WriteLine($"Метод: {_method.Name}");
        Console.WriteLine();

        Console.WriteLine("Шаги алгоритма:");
        Console.WriteLine("┌─────┬─────────┬─────────┬─────────┬─────────┬─────────┬─────────┬─────────┐");
        Console.WriteLine("│  k  │    ak   │    bk   │    y    │    z    │  f(y)   │  f(z)   │    Δ    │");
        Console.WriteLine("├─────┼─────────┼─────────┼─────────┼─────────┼─────────┼─────────┼─────────┤");

        foreach (var step in result.Steps)
        {
            Console.WriteLine($"│ {step.Iteration,3} │ {step.A,7:F3} │ {step.B,7:F3} │ {step.Y,7:F3} │ {step.Z,7:F3} │ {step.FY,7:F2} │ {step.FZ,7:F2} │ {step.IntervalLength,7:F3} │");
        }

        Console.WriteLine("└─────┴─────────┴─────────┴─────────┴─────────┴─────────┴─────────┴─────────┘");
        Console.WriteLine();

        Console.WriteLine("=== Результат ===");
        Console.WriteLine($"x* = {result.OptimalX:F4}");
        Console.WriteLine($"f(x*) = {result.OptimalValue:F4}");
        Console.WriteLine($"k = {result.Steps.Count}");
    }
}

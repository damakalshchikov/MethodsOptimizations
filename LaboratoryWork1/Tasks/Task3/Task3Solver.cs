using LaboratoryWork1.Core;
using LaboratoryWork1.Models;

namespace LaboratoryWork1.Tasks.Task3;

// Решатель для задания №3
public class Task3Solver
{
    private readonly IFunction _function;
    private readonly IOptimizationMethod _method;

    public Task3Solver()
    {
        _function = new Task3Function();
        _method = new FibonacciMethod();
    }

    public OptimizationResult Solve()
    {
        // Параметры из задания
        double a = -1;
        double b = 3;
        double l = 0.3; // Допустимая длина конечного интервала

        return _method.FindMinimum(_function, a, b, l);
    }

    public void PrintResult(OptimizationResult result)
    {
        Console.WriteLine("=== Задание №3 ===");
        Console.WriteLine(_function.Name);
        Console.WriteLine($"Интервал: [-1; 3]");
        Console.WriteLine($"Длина конечного интервала: l = 0.3");
        Console.WriteLine($"Константа различимости: ε = 0.15");
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

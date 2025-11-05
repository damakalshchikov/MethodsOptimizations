using LaboratoryWork2.Core;
using LaboratoryWork2.Models;

namespace LaboratoryWork2.Tasks.Task2;

// Решатель для задачи 2 (метод наискорейшего градиентного спуска)
public class Task2Solver
{
    private readonly IFunction _function;
    private readonly OptimizationParameters _parameters;

    public Task2Solver()
    {
        _function = new Task2Function();

        // Параметры из задания:
        // x⁰ = (0; 0,5)
        // ε₁ = 0,15
        // ε₂ = 0,2
        // M = 10
        _parameters = new OptimizationParameters
        {
            Epsilon1 = 0.15,
            Epsilon2 = 0.2,
            MaxIterations = 10,
            InitialStepSize = 1.0,  // Начальный диапазон для поиска оптимального шага
            StrongDescentParameter = 0.1
        };
    }

    public OptimizationResult Solve()
    {
        double[] initialPoint = new[] { 0.0, 0.5 };
        var method = new SteepestGradientDescent();
        return method.Optimize(_function, initialPoint, _parameters);
    }

    public void PrintResult(OptimizationResult result)
    {
        Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║ Метод наискорейшего градиентного спуска (Задача 2)             ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        Console.WriteLine("Параметры задачи:");
        Console.WriteLine("  x⁰ = (0,0; 0,5)");
        Console.WriteLine("  ε₁ = 0,15 (критерий по градиенту)");
        Console.WriteLine("  ε₂ = 0,2 (критерий по изменению точки и функции)");
        Console.WriteLine("  M = 10 (максимум итераций)");
        Console.WriteLine();

        Console.WriteLine("Таблица итераций:");
        Console.WriteLine("┌────┬──────────────┬──────────────┬──────────────┬──────────────┐");
        Console.WriteLine("│  k │     x₁       │     x₂       │   f(x)       │  ||∇f(x)||   │");
        Console.WriteLine("├────┼──────────────┼──────────────┼──────────────┼──────────────┤");

        foreach (var step in result.Steps)
        {
            Console.WriteLine(
                $"│{step.Iteration:D2}  │ {step.X[0],12:F8} │ {step.X[1],12:F8} │ {step.FunctionValue,12:F8} │ {step.GradientNorm,12:F8} │"
            );
        }

        Console.WriteLine("└────┴──────────────┴──────────────┴──────────────┴──────────────┘");
        Console.WriteLine();

        Console.WriteLine("Результаты оптимизации:");
        Console.WriteLine($"  x* = ({result.OptimalPoint[0]:F8}; {result.OptimalPoint[1]:F8})");
        Console.WriteLine($"  f(x*) = {result.OptimalValue:F8}");
        Console.WriteLine($"  k = {result.IterationCount}");
        Console.WriteLine();
    }
}

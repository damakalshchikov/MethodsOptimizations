using LaboratoryWork3.Core;
using LaboratoryWork3.Models;

namespace LaboratoryWork3.Tasks.Task1;

// Решатель для задачи 1 (метод Ньютона)
public class Task1Solver
{
    private readonly IFunction _function;
    private readonly OptimizationParameters _parameters;

    public Task1Solver()
    {
        _function = new Task1Function();

        // Параметры из задания:
        // x⁰ = (1,5; 0,5)
        // ε₁ = 0,15
        // ε₂ = 0,2
        // M = 10
        _parameters = new OptimizationParameters
        {
            Epsilon1 = 0.15,
            Epsilon2 = 0.2,
            MaxIterations = 10,
            InitialStepSize = 0.5,
            StrongDescentParameter = 0.1
        };
    }

    public OptimizationResult Solve()
    {
        double[] initialPoint = new[] { 1.5, 0.5 };
        var method = new NewtonMethod();
        return method.Optimize(_function, initialPoint, _parameters);
    }

    public void PrintResult(OptimizationResult result)
    {
        Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║ Метод Ньютона для поиска минимума                              ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        Console.WriteLine("Параметры задачи:");
        Console.WriteLine("  x⁰ = (1,5; 0,5)");
        Console.WriteLine("  ε₁ = 0,15");
        Console.WriteLine("  ε₂ = 0,2");
        Console.WriteLine("  M = 10");
        Console.WriteLine();

        Console.WriteLine("Таблица итераций:");
        Console.WriteLine("┌────┬──────────────┬──────────────┬──────────────┬──────────────┐");
        Console.WriteLine("│  k │     x₁       │     x₂       │   f(x)       │  ||∇f(x)||   │");
        Console.WriteLine("├────┼──────────────┼──────────────┼──────────────┼──────────────┤");

        foreach (var step in result.Steps)
        {
            Console.WriteLine(
                $"│  {step.Iteration} │ {step.X[0],12:F8} │ {step.X[1],12:F8} │ {step.FunctionValue,12:F8} │ {step.GradientNorm,12:F8} │"
            );
        }

        Console.WriteLine("└────┴──────────────┴──────────────┴──────────────┴──────────────┘");
        Console.WriteLine();

        Console.WriteLine("Результаты:");
        Console.WriteLine($"  x* = ({result.OptimalPoint[0]:F8}; {result.OptimalPoint[1]:F8})");
        Console.WriteLine($"  f(x*) = {result.OptimalValue:F8}");
        Console.WriteLine($"  k = {result.IterationCount}");
        Console.WriteLine();
    }
}

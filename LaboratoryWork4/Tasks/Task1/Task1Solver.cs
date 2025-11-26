using LaboratoryWork4.Core;
using LaboratoryWork4.Models;

namespace LaboratoryWork4.Tasks.Task1;

public class Task1Solver
{
    public OptimizationResult Solve()
    {
        // Параметры задачи из условия
        double[] initialPoint = new[] { 0.0, 0.0 }; // Начальная точка
        double initialPenalty = 0.5; // r⁰ = 0.5
        double penaltyMultiplier = 8.0; // C = 8
        double epsilon = 0.05; // ε = 0.05

        // Создать функцию задачи
        var function = new Task1Function();

        // Создать метод безусловной оптимизации (метод Ньютона)
        var newtonMethod = new NewtonMethod();

        // Создать метод штрафов
        var penaltyMethod = new PenaltyMethod(function, newtonMethod);

        // Параметры метода штрафов
        var parameters = new PenaltyMethodParameters
        {
            InitialPoint = initialPoint,
            InitialPenalty = initialPenalty,
            PenaltyMultiplier = penaltyMultiplier,
            Epsilon = epsilon,
            MaxIterations = 100
        };

        // Решить задачу
        var result = penaltyMethod.Solve(parameters);

        return result;
    }

    public void PrintResult(OptimizationResult result)
    {
        Console.WriteLine("Метод штрафов");
        Console.WriteLine("=============");
        Console.WriteLine();

        Console.WriteLine("Данные:");
        Console.WriteLine("f(x) = 3x₁² + 2x₂² - 10 → min");
        Console.WriteLine("g₁(x) = x₁ + 2x₂ - 6 = 0");
        Console.WriteLine("ε = 0.05");
        Console.WriteLine("r⁰ = 0.5");
        Console.WriteLine("C = 8");
        Console.WriteLine();

        // Таблица итераций
        Console.WriteLine("Итерации метода штрафов:");
        Console.WriteLine("┌─────┬──────────────┬──────────────┬──────────────┬──────────────┬──────────────┐");
        Console.WriteLine("│  k  │      x₁      │      x₂      │     f(x)     │     P(x)     │      r       │");
        Console.WriteLine("├─────┼──────────────┼──────────────┼──────────────┼──────────────┼──────────────┤");

        foreach (var step in result.Steps)
        {
            Console.WriteLine($"│ {step.Iteration,3} │ {step.X[0],12:F6} │ {step.X[1],12:F6} │ {step.FunctionValue,12:F6} │ {step.PenaltyValue,12:F6} │ {step.R,12:F6} │");
        }

        Console.WriteLine("└─────┴──────────────┴──────────────┴──────────────┴──────────────┴──────────────┘");
        Console.WriteLine();

        // Финальные результаты
        Console.WriteLine("Результаты:");
        Console.WriteLine($"x* = ({result.OptimalPoint[0]:F6}, {result.OptimalPoint[1]:F6})");
        Console.WriteLine($"f(x*) = {result.OptimalValue:F6}");
        Console.WriteLine($"k = {result.IterationCount}");
        Console.WriteLine();
    }
}

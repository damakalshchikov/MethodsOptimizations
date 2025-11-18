namespace LaboratoryWork3.Models;

// Результат работы оптимизационного алгоритма
public class OptimizationResult
{
    public required double[] OptimalPoint { get; set; }
    public double OptimalValue { get; set; }
    public int IterationCount { get; set; }
    public List<OptimizationStep> Steps { get; set; } = new();
}

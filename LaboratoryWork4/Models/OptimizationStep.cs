namespace LaboratoryWork4.Models;

// Представляет одну итерацию оптимизационного алгоритма
public class OptimizationStep
{
    public int Iteration { get; set; }
    public required double[] X { get; set; }
    public double FunctionValue { get; set; }
    public double PenaltyValue { get; set; }
    public double R { get; set; }
}

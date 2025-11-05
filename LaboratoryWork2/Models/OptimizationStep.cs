namespace LaboratoryWork2.Models;

// Представляет одну итерацию оптимизационного алгоритма
public class OptimizationStep
{
    public int Iteration { get; set; }
    public required double[] X { get; set; }
    public required double[] Gradient { get; set; }
    public double FunctionValue { get; set; }
    public double StepSize { get; set; }
    public double GradientNorm { get; set; }
}

using LaboratoryWork2.Core;

namespace LaboratoryWork2.Tasks.Task2;

// Функция для задачи 2 (та же функция, что и в Task1)
// f(x) = 6x₁² + 0,4x₁x₂ + 5x₂²
public class Task2Function : IFunction
{
    public double Evaluate(double[] x)
    {
        double x1 = x[0];
        double x2 = x[1];

        // f(x) = 6x₁² + 0,4x₁x₂ + 5x₂²
        return 6 * x1 * x1 + 0.4 * x1 * x2 + 5 * x2 * x2;
    }

    public double[] Gradient(double[] x)
    {
        double x1 = x[0];
        double x2 = x[1];

        // ∂f/∂x₁ = 12x₁ + 0,4x₂
        // ∂f/∂x₂ = 0,4x₁ + 10x₂
        return new[]
        {
            12 * x1 + 0.4 * x2,
            0.4 * x1 + 10 * x2
        };
    }
}

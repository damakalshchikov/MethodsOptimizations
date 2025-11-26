using LaboratoryWork4.Core;

namespace LaboratoryWork4.Tasks.Task1;

// Функция для задачи 1:
// f(x) = 3x₁² + 2x₂² - 10
// g₁(x) = x₁ + 2x₂ - 6 = 0
public class Task1Function : IConstrainedFunction
{
    // Целевая функция: f(x) = 3x₁² + 2x₂² - 10
    public double ObjectiveFunction(double[] x)
    {
        return 3 * x[0] * x[0] + 2 * x[1] * x[1] - 10;
    }

    // Градиент целевой функции
    // ∂f/∂x₁ = 6x₁
    // ∂f/∂x₂ = 4x₂
    public double[] ObjectiveFunctionGradient(double[] x)
    {
        return new[]
        {
            6 * x[0],
            4 * x[1]
        };
    }

    // Гессиан (матрица вторых производных) целевой функции
    // ∂²f/∂x₁² = 6
    // ∂²f/∂x₂² = 4
    // ∂²f/∂x₁∂x₂ = 0
    public double[][] ObjectiveFunctionHessian(double[] x)
    {
        return new[]
        {
            new[] { 6.0, 0.0 },
            new[] { 0.0, 4.0 }
        };
    }

    // Ограничения равенства: g₁(x) = x₁ + 2x₂ - 6 = 0
    public double[] EqualityConstraints(double[] x)
    {
        return new[]
        {
            x[0] + 2 * x[1] - 6
        };
    }

    // Ограничения неравенства (в данной задаче их нет)
    public double[] InequalityConstraints(double[] x)
    {
        return Array.Empty<double>();
    }
}

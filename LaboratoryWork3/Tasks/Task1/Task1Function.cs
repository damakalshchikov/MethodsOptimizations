using LaboratoryWork3.Core;

namespace LaboratoryWork3.Tasks.Task1;

// Функция для задачи 1 (метод Ньютона)
// f(x) = 6x₁² + 0,4x₁x₂ + 5x₂²
public class Task1Function : IFunction
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

    public double[][] Hessian(double[] x)
    {
        // Матрица Гессе (вторые производные)
        // Для квадратичной функции Гессе постоянна:
        // H = [∂²f/∂x₁²    ∂²f/∂x₁∂x₂  ]   [12    0,4]
        //     [∂²f/∂x₂∂x₁  ∂²f/∂x₂²    ] = [0,4   10 ]

        return new[]
        {
            new[] { 12.0, 0.4 },
            new[] { 0.4, 10.0 }
        };
    }
}

using LaboratoryWork4.Models;

namespace LaboratoryWork4.Core;

// Метод Ньютона для поиска минимума функции
public class NewtonMethod : IOptimizationMethod
{
    public OptimizationResult Optimize(IFunction function, double[] initialPoint, OptimizationParameters parameters)
    {
        int n = initialPoint.Length;
        double[] x = (double[])initialPoint.Clone();
        int k = 0;

        var result = new OptimizationResult { OptimalPoint = x };

        while (true)
        {
            // Вычислить градиент и значение функции
            double[] gradient = function.Gradient(x);
            double fValue = function.Evaluate(x);
            double gradientNorm = MatrixUtils.VectorNorm(gradient);

            // Проверить критерий окончания ||∇f(x^k)|| < ε₁
            if (gradientNorm < parameters.Epsilon1)
            {
                result.OptimalPoint = x;
                result.OptimalValue = fValue;
                result.IterationCount = k + 1;
                return result;
            }

            // Проверить максимальное число итераций
            if (k >= parameters.MaxIterations)
            {
                result.OptimalPoint = x;
                result.OptimalValue = fValue;
                result.IterationCount = k + 1;
                return result;
            }

            // Вычислить матрицу Гессе
            double[][] hessian = function.Hessian(x);

            double[] direction;

            try
            {
                // Инвертировать Гессе
                double[][] hessianInv = MatrixUtils.Invert2x2(hessian);

                // Проверить, положительно ли определена обратная матрица
                if (MatrixUtils.IsPositiveDefinite(hessianInv))
                {
                    // d^k = -H^{-1}(x^k)∇f(x^k)
                    double[] gradientNegative = new double[n];
                    for (int i = 0; i < n; i++)
                        gradientNegative[i] = -gradient[i];

                    direction = MatrixUtils.MatrixVectorMultiply(hessianInv, gradientNegative);

                    // x^{k+1} = x^k + d^k
                    double[] xNext = new double[n];
                    for (int i = 0; i < n; i++)
                    {
                        xNext[i] = x[i] + direction[i];
                    }

                    // Проверить, улучшилась ли функция
                    double fNext = function.Evaluate(xNext);
                    if (fNext < fValue)
                    {
                        x = xNext;
                    }
                    else
                    {
                        // Использовать градиентный спуск
                        x = GradientDescent(function, x, gradient, fValue, parameters.InitialStepSize);
                    }
                }
                else
                {
                    // Использовать градиентный спуск
                    x = GradientDescent(function, x, gradient, fValue, parameters.InitialStepSize);
                }
            }
            catch
            {
                // Если матрица не может быть инвертирована, использовать градиентный спуск
                x = GradientDescent(function, x, gradient, fValue, parameters.InitialStepSize);
            }

            k++;
        }
    }

    // Градиентный спуск с поиском шага
    private static double[] GradientDescent(IFunction function, double[] x, double[] gradient, double fValue, double initialStep)
    {
        int n = x.Length;
        double[] direction = new double[n];
        for (int i = 0; i < n; i++)
            direction[i] = -gradient[i];

        double t = initialStep;
        while (true)
        {
            double[] xNext = new double[n];
            for (int i = 0; i < n; i++)
            {
                xNext[i] = x[i] + t * direction[i];
            }
            double fNext = function.Evaluate(xNext);

            if (fNext < fValue)
                return xNext;

            t = t / 2;

            // Защита от бесконечного цикла
            if (t < 1e-10)
                return xNext;
        }
    }
}

using LaboratoryWork2.Models;

namespace LaboratoryWork2.Core;

// Метод наискорейшего градиентного спуска (градиентный спуск с оптимальным шагом)
public class SteepestGradientDescent : IOptimizationMethod
{
    public OptimizationResult Optimize(IFunction function, double[] initialPoint, OptimizationParameters parameters)
    {
        int n = initialPoint.Length;
        double[] x = (double[])initialPoint.Clone();
        int k = 0;

        var result = new OptimizationResult { OptimalPoint = x };
        var steps = result.Steps;

        // Флаг для отслеживания выполнения условий в предыдущей итерации
        bool prevConditionsMetFlag = false;

        // Шаг 1: задать x^0, параметры (делается в параметрах)
        // Шаг 2: положить k = 0

        while (true)
        {
            // Шаг 3: вычислить ∇f(x^k)
            double[] gradient = function.Gradient(x);
            double fValue = function.Evaluate(x);
            double gradientNorm = VectorNorm(gradient);

            // Записать шаг итерации
            var step = new OptimizationStep
            {
                Iteration = k,
                X = (double[])x.Clone(),
                Gradient = (double[])gradient.Clone(),
                FunctionValue = fValue,
                GradientNorm = gradientNorm
            };
            steps.Add(step);

            // Шаг 4: проверить критерий окончания ||∇f(x^k)|| < ε₁
            if (gradientNorm < parameters.Epsilon1)
            {
                result.OptimalPoint = x;
                result.OptimalValue = fValue;
                result.IterationCount = k + 1;
                return result;
            }

            // Шаг 5: проверить k ≥ M
            if (k >= parameters.MaxIterations)
            {
                result.OptimalPoint = x;
                result.OptimalValue = fValue;
                result.IterationCount = k + 1;
                return result;
            }

            // Шаг 6: найти оптимальный шаг t_k путем минимизации φ(t) = f(x^k - t∇f(x^k))
            double optimalStepSize = FindOptimalStepSize(function, x, gradient, parameters.InitialStepSize);

            // Шаг 7: вычислить x^{k+1} = x^k - t_k^* ∇f(x^k)
            double[] xNext = new double[n];
            for (int i = 0; i < n; i++)
            {
                xNext[i] = x[i] - optimalStepSize * gradient[i];
            }
            double fNext = function.Evaluate(xNext);

            // Шаг 8: проверить условия окончания
            double[] diff = new double[n];
            for (int i = 0; i < n; i++)
            {
                diff[i] = xNext[i] - x[i];
            }
            double diffNorm = VectorNorm(diff);
            double fDiff = Math.Abs(fNext - fValue);

            // Проверить оба условия: ||x^{k+1} - x^k|| < ε₂ и |f(x^{k+1}) - f(x^k)| < ε₂
            bool currentConditionsMet = (diffNorm < parameters.Epsilon2 && fDiff < parameters.Epsilon2);

            if (currentConditionsMet && prevConditionsMetFlag)
            {
                // Оба условия выполнены при текущем k И при k-1
                // Расчет завершен
                result.OptimalPoint = xNext;
                result.OptimalValue = fNext;
                result.IterationCount = k + 1;
                return result;
            }

            // Обновить x для следующей итерации
            x = xNext;
            prevConditionsMetFlag = currentConditionsMet;
            k++;
        }
    }

    // Найти оптимальный шаг путем минимизации функции одной переменной
    private static double FindOptimalStepSize(IFunction function, double[] x, double[] gradient, double maxT)
    {
        int n = x.Length;

        // Функция одной переменной: φ(t) = f(x - t∇f(x))
        Func<double, double> phi = (double t) =>
        {
            double[] xNext = new double[n];
            for (int i = 0; i < n; i++)
            {
                xNext[i] = x[i] - t * gradient[i];
            }
            return function.Evaluate(xNext);
        };

        // Используем метод золотого сечения для поиска минимума на [0, maxT]
        double optimalT = OneVariableOptimizer.Minimize(phi, maxT, 1e-8);

        return optimalT;
    }

    // Вычислить евклидову норму вектора
    private static double VectorNorm(double[] vector)
    {
        double sum = 0;
        foreach (var v in vector)
        {
            sum += v * v;
        }
        return Math.Sqrt(sum);
    }
}

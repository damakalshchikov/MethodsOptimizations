using LaboratoryWork2.Models;

namespace LaboratoryWork2.Core;

// Метод градиентного спуска с постоянным (адаптивным) шагом
public class GradientDescentConstantStep : IOptimizationMethod
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
                result.IterationCount = k;
                return result;
            }

            // Шаг 5: проверить k ≥ M
            if (k >= parameters.MaxIterations)
            {
                result.OptimalPoint = x;
                result.OptimalValue = fValue;
                result.IterationCount = k;
                return result;
            }

            // Шаг 6: задать величину шага t_k
            double t = parameters.InitialStepSize;

            // Шаг 7-8: цикл поиска подходящего шага
            double[] xNext;
            double fNext;
            while (true)
            {
                // Шаг 7: вычислить x^{k+1} = x^k - t_k ∇f(x^k)
                xNext = new double[n];
                for (int i = 0; i < n; i++)
                {
                    xNext[i] = x[i] - t * gradient[i];
                }
                fNext = function.Evaluate(xNext);

                // Шаг 8: проверить условие f(x^{k+1}) - f(x^k) < 0
                // (или альтернативное условие с сильным убыванием)
                if (fNext - fValue < 0 ||
                    fNext - fValue < -parameters.StrongDescentParameter * t * VectorDotProduct(gradient, gradient))
                {
                    break; // Условие выполнено, переходим к шагу 9
                }

                // Условие не выполнено: t_k = t_k / 2
                t = t / 2;
            }

            // Шаг 9: проверить условия окончания
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

    // Вычислить скалярное произведение
    private static double VectorDotProduct(double[] a, double[] b)
    {
        double sum = 0;
        for (int i = 0; i < a.Length; i++)
        {
            sum += a[i] * b[i];
        }
        return sum;
    }
}

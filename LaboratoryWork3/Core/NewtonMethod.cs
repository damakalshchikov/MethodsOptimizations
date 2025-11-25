using LaboratoryWork3.Models;

namespace LaboratoryWork3.Core;

// Метод Ньютона для поиска минимума функции
public class NewtonMethod : IOptimizationMethod
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

        // Шаг 1: задать x^0, параметры
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

            // Шаг 6: вычислить матрицу Гессе H(x^k)
            double[][] hessian = function.Hessian(x);

            // Шаг 7: вычислить обратную матрицу H^{-1}(x^k)
            double[][] hessianInv;
            double[] direction;

            try
            {
                hessianInv = MatrixUtils.Invert2x2(hessian);

                // Шаг 8: проверить, положительно ли определена H^{-1}
                if (MatrixUtils.IsPositiveDefinite(hessianInv))
                {
                    // Шаг 9: d^k = -H^{-1}(x^k)∇f(x^k)
                    double[] gradientNegative = new double[n];
                    for (int i = 0; i < n; i++)
                        gradientNegative[i] = -gradient[i];

                    direction = MatrixUtils.MatrixVectorMultiply(hessianInv, gradientNegative);

                    // Шаг 10: x^{k+1} = x^k + t_k * d^k с t_k = 1
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
                        // Если не улучшилась, использовать градиентный спуск с поиском шага
                        direction = new double[n];
                        for (int i = 0; i < n; i++)
                            direction[i] = -gradient[i];

                        // Поиск подходящего шага
                        double t = parameters.InitialStepSize;
                        while (true)
                        {
                            xNext = new double[n];
                            for (int i = 0; i < n; i++)
                            {
                                xNext[i] = x[i] + t * direction[i];
                            }
                            fNext = function.Evaluate(xNext);

                            if (fNext < fValue)
                                break;

                            t = t / 2;
                        }

                        x = xNext;
                    }
                }
                else
                {
                    // Шаг 8б: если H^{-1} не положительно определена, использовать градиент
                    direction = new double[n];
                    for (int i = 0; i < n; i++)
                        direction[i] = -gradient[i];

                    // Поиск подходящего шага
                    double t = parameters.InitialStepSize;
                    double[] xNext;
                    double fNext;

                    while (true)
                    {
                        xNext = new double[n];
                        for (int i = 0; i < n; i++)
                        {
                            xNext[i] = x[i] + t * direction[i];
                        }
                        fNext = function.Evaluate(xNext);

                        if (fNext < fValue)
                        {
                            x = xNext;
                            break;
                        }

                        t = t / 2;
                    }
                }
            }
            catch
            {
                // Если матрица не может быть инвертирована, использовать градиентный спуск
                direction = new double[n];
                for (int i = 0; i < n; i++)
                    direction[i] = -gradient[i];

                double t = parameters.InitialStepSize;
                double[] xNext;
                double fNext;

                while (true)
                {
                    xNext = new double[n];
                    for (int i = 0; i < n; i++)
                    {
                        xNext[i] = x[i] + t * direction[i];
                    }
                    fNext = function.Evaluate(xNext);

                    if (fNext < fValue)
                    {
                        x = xNext;
                        break;
                    }

                    t = t / 2;
                }
            }

            // Вычислить новую функцию и разницу
            double fNew = function.Evaluate(x);

            // Шаг 11: проверить условия окончания
            double[] diff = new double[n];
            for (int i = 0; i < n; i++)
            {
                diff[i] = x[i] - steps[k].X[i];
            }
            double diffNorm = VectorNorm(diff);
            double fDiff = Math.Abs(fNew - fValue);

            bool currentConditionsMet = (diffNorm < parameters.Epsilon2 && fDiff < parameters.Epsilon2);

            if (currentConditionsMet && prevConditionsMetFlag)
            {
                // Условия выполнены при текущем k И при k-1
                result.OptimalPoint = x;
                result.OptimalValue = fNew;
                result.IterationCount = k + 2;
                return result;
            }

            prevConditionsMetFlag = currentConditionsMet;
            k++;
        }
    }

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

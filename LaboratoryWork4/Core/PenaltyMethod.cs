using LaboratoryWork4.Models;

namespace LaboratoryWork4.Core;

// Параметры метода штрафов
public class PenaltyMethodParameters
{
    // Начальная точка
    public required double[] InitialPoint { get; set; }

    // Начальное значение параметра штрафа
    public double InitialPenalty { get; set; }

    // Коэффициент увеличения параметра штрафа
    public double PenaltyMultiplier { get; set; }

    // Точность остановки
    public double Epsilon { get; set; }

    // Максимальное число итераций
    public int MaxIterations { get; set; } = 100;
}

// Интерфейс для задачи оптимизации с ограничениями
public interface IConstrainedFunction
{
    // Целевая функция f(x)
    double ObjectiveFunction(double[] x);

    // Градиент целевой функции
    double[] ObjectiveFunctionGradient(double[] x);

    // Гессиан целевой функции
    double[][] ObjectiveFunctionHessian(double[] x);

    // Ограничения равенства g_j(x) = 0, j = 1..m
    double[] EqualityConstraints(double[] x);

    // Ограничения неравенства g_j(x) <= 0, j = m+1..p
    double[] InequalityConstraints(double[] x);
}

// Метод штрафов для решения задач условной оптимизации
public class PenaltyMethod
{
    private readonly IConstrainedFunction _constrainedFunction;
    private readonly IOptimizationMethod _unconstrainedMethod;

    public PenaltyMethod(IConstrainedFunction constrainedFunction, IOptimizationMethod unconstrainedMethod)
    {
        _constrainedFunction = constrainedFunction;
        _unconstrainedMethod = unconstrainedMethod;
    }

    public OptimizationResult Solve(PenaltyMethodParameters parameters)
    {
        // Шаг 1: Инициализация
        double[] x = (double[])parameters.InitialPoint.Clone();
        double r = parameters.InitialPenalty;
        int k = 0;

        var result = new OptimizationResult { OptimalPoint = x };
        var steps = result.Steps;

        while (k < parameters.MaxIterations)
        {
            // Шаг 2: Формирование вспомогательной функции F(x, r^k)
            var auxiliaryFunction = new AuxiliaryFunction(_constrainedFunction, r);

            // Шаг 3: Поиск безусловного минимума
            var unconstrainedParams = new OptimizationParameters
            {
                Epsilon1 = 1e-6,
                Epsilon2 = 1e-6,
                MaxIterations = 1000,
                InitialStepSize = 0.5
            };

            var unconstrainedResult = _unconstrainedMethod.Optimize(
                auxiliaryFunction,
                x,
                unconstrainedParams
            );

            double[] xStar = unconstrainedResult.OptimalPoint;
            double fValue = _constrainedFunction.ObjectiveFunction(xStar);

            // Вычислить значение штрафной функции P(x*, r^k)
            double penaltyValue = CalculatePenalty(xStar, r);

            // Записать шаг
            var step = new OptimizationStep
            {
                Iteration = k,
                X = (double[])xStar.Clone(),
                FunctionValue = fValue,
                PenaltyValue = penaltyValue,
                R = r
            };
            steps.Add(step);

            // Шаг 4: Проверка условия окончания
            if (penaltyValue <= parameters.Epsilon)
            {
                result.OptimalPoint = xStar;
                result.OptimalValue = fValue;
                result.IterationCount = k + 1;
                return result;
            }

            // Обновить параметры для следующей итерации
            r = parameters.PenaltyMultiplier * r;
            x = xStar;
            k++;
        }

        // Если достигнуто максимальное число итераций
        result.OptimalPoint = x;
        result.OptimalValue = _constrainedFunction.ObjectiveFunction(x);
        result.IterationCount = k;
        return result;
    }

    // Вычислить штрафную функцию P(x, r)
    private double CalculatePenalty(double[] x, double r)
    {
        double penalty = 0;

        // Ограничения равенства: [g_j(x)]^2
        double[] equalityConstraints = _constrainedFunction.EqualityConstraints(x);
        foreach (var g in equalityConstraints)
        {
            penalty += g * g;
        }

        // Ограничения неравенства: [g_j^+(x)]^2, где g_j^+ = max(0, g_j)
        double[] inequalityConstraints = _constrainedFunction.InequalityConstraints(x);
        foreach (var g in inequalityConstraints)
        {
            double gPlus = Math.Max(0, g);
            penalty += gPlus * gPlus;
        }

        return (r / 2.0) * penalty;
    }

    // Вспомогательная функция F(x, r) = f(x) + P(x, r)
    private class AuxiliaryFunction : IFunction
    {
        private readonly IConstrainedFunction _constrainedFunc;
        private readonly double _r;

        public AuxiliaryFunction(IConstrainedFunction constrainedFunc, double r)
        {
            _constrainedFunc = constrainedFunc;
            _r = r;
        }

        public double Evaluate(double[] x)
        {
            double f = _constrainedFunc.ObjectiveFunction(x);
            double p = CalculatePenalty(x);
            return f + p;
        }

        public double[] Gradient(double[] x)
        {
            // Градиент вспомогательной функции = градиент f(x) + градиент P(x, r)
            double[] gradF = _constrainedFunc.ObjectiveFunctionGradient(x);
            double[] gradP = CalculatePenaltyGradient(x);

            int n = x.Length;
            double[] result = new double[n];
            for (int i = 0; i < n; i++)
            {
                result[i] = gradF[i] + gradP[i];
            }

            return result;
        }

        public double[][] Hessian(double[] x)
        {
            // Гессиан вспомогательной функции = Гессиан f(x) + Гессиан P(x, r)
            double[][] hessF = _constrainedFunc.ObjectiveFunctionHessian(x);
            double[][] hessP = CalculatePenaltyHessian(x);

            int n = x.Length;
            double[][] result = new double[n][];
            for (int i = 0; i < n; i++)
            {
                result[i] = new double[n];
                for (int j = 0; j < n; j++)
                {
                    result[i][j] = hessF[i][j] + hessP[i][j];
                }
            }

            return result;
        }

        private double CalculatePenalty(double[] x)
        {
            double penalty = 0;

            // Ограничения равенства
            double[] equalityConstraints = _constrainedFunc.EqualityConstraints(x);
            foreach (var g in equalityConstraints)
            {
                penalty += g * g;
            }

            // Ограничения неравенства
            double[] inequalityConstraints = _constrainedFunc.InequalityConstraints(x);
            foreach (var g in inequalityConstraints)
            {
                double gPlus = Math.Max(0, g);
                penalty += gPlus * gPlus;
            }

            return (_r / 2.0) * penalty;
        }

        private double[] CalculatePenaltyGradient(double[] x)
        {
            int n = x.Length;
            double[] grad = new double[n];

            // Для простоты используем численное дифференцирование
            double h = 1e-6;
            for (int i = 0; i < n; i++)
            {
                double[] xPlusH = (double[])x.Clone();
                xPlusH[i] += h;

                double[] xMinusH = (double[])x.Clone();
                xMinusH[i] -= h;

                double pPlus = CalculatePenalty(xPlusH);
                double pMinus = CalculatePenalty(xMinusH);

                grad[i] = (pPlus - pMinus) / (2 * h);
            }

            return grad;
        }

        private double[][] CalculatePenaltyHessian(double[] x)
        {
            int n = x.Length;
            double[][] hess = new double[n][];

            // Для простоты используем численное дифференцирование
            double h = 1e-5;
            for (int i = 0; i < n; i++)
            {
                hess[i] = new double[n];
                for (int j = 0; j < n; j++)
                {
                    double[] xPlusIJ = (double[])x.Clone();
                    xPlusIJ[i] += h;
                    xPlusIJ[j] += h;

                    double[] xPlusI = (double[])x.Clone();
                    xPlusI[i] += h;

                    double[] xPlusJ = (double[])x.Clone();
                    xPlusJ[j] += h;

                    double pPlusIJ = CalculatePenalty(xPlusIJ);
                    double pPlusI = CalculatePenalty(xPlusI);
                    double pPlusJ = CalculatePenalty(xPlusJ);
                    double p = CalculatePenalty(x);

                    hess[i][j] = (pPlusIJ - pPlusI - pPlusJ + p) / (h * h);
                }
            }

            return hess;
        }
    }
}

using LaboratoryWork4.Models;

namespace LaboratoryWork4.Core;

// Параметры метода барьерных функций
public class BarrierMethodParameters
{
    // Начальная точка (должна быть внутри допустимой области: g_j(x) < 0)
    public required double[] InitialPoint { get; set; }

    // Начальное значение параметра барьера
    public double InitialBarrier { get; set; }

    // Коэффициент уменьшения параметра барьера (C > 1)
    public double BarrierDivisor { get; set; }

    // Точность остановки
    public double Epsilon { get; set; }

    // Максимальное число итераций
    public int MaxIterations { get; set; } = 100;

    // Тип барьерной функции
    public BarrierFunctionType BarrierType { get; set; } = BarrierFunctionType.Logarithmic;
}

// Тип барьерной функции
public enum BarrierFunctionType
{
    Inverse,      // Обратная: P(x, r) = -r * Σ(1/g_j(x))
    Logarithmic   // Логарифмическая: P(x, r) = -r * Σ ln(-g_j(x))
}

// Интерфейс для задачи оптимизации с ограничениями неравенства
public interface IInequalityConstrainedFunction
{
    // Целевая функция f(x)
    double ObjectiveFunction(double[] x);

    // Градиент целевой функции
    double[] ObjectiveFunctionGradient(double[] x);

    // Гессиан целевой функции
    double[][] ObjectiveFunctionHessian(double[] x);

    // Ограничения неравенства g_j(x) <= 0
    double[] InequalityConstraints(double[] x);
}

// Метод барьерных функций для решения задач с ограничениями неравенства
public class BarrierMethod
{
    private readonly IInequalityConstrainedFunction _constrainedFunction;
    private readonly IOptimizationMethod _unconstrainedMethod;

    public BarrierMethod(IInequalityConstrainedFunction constrainedFunction, IOptimizationMethod unconstrainedMethod)
    {
        _constrainedFunction = constrainedFunction;
        _unconstrainedMethod = unconstrainedMethod;
    }

    public OptimizationResult Solve(BarrierMethodParameters parameters)
    {
        // Шаг 1: Инициализация
        double[] x = (double[])parameters.InitialPoint.Clone();
        double r = parameters.InitialBarrier;
        int k = 0;

        // Проверить, что начальная точка внутри допустимой области
        if (!IsInsideFeasibleRegion(x))
        {
            throw new ArgumentException("Начальная точка должна быть внутри допустимой области (g_j(x) < 0 для всех j)");
        }

        var result = new OptimizationResult { OptimalPoint = x };
        var steps = result.Steps;

        while (k < parameters.MaxIterations)
        {
            // Шаг 2: Формирование вспомогательной функции F(x, r^k)
            var auxiliaryFunction = new AuxiliaryBarrierFunction(
                _constrainedFunction,
                r,
                parameters.BarrierType
            );

            // Шаг 3: Поиск безусловного минимума
            var unconstrainedParams = new OptimizationParameters
            {
                Epsilon1 = 1e-6,
                Epsilon2 = 1e-6,
                MaxIterations = 1000,
                InitialStepSize = 0.1  // Меньший шаг для барьерного метода
            };

            var unconstrainedResult = _unconstrainedMethod.Optimize(
                auxiliaryFunction,
                x,
                unconstrainedParams
            );

            double[] xStar = unconstrainedResult.OptimalPoint;

            // Проверить, что точка осталась внутри допустимой области
            if (!IsInsideFeasibleRegion(xStar))
            {
                // Если точка вышла за границы, попробовать с меньшим шагом
                unconstrainedParams.InitialStepSize = 0.01;
                unconstrainedResult = _unconstrainedMethod.Optimize(
                    auxiliaryFunction,
                    x,
                    unconstrainedParams
                );
                xStar = unconstrainedResult.OptimalPoint;
            }

            double fValue = _constrainedFunction.ObjectiveFunction(xStar);

            // Вычислить значение барьерной функции P(x*, r^k)
            double barrierValue = CalculateBarrier(xStar, r, parameters.BarrierType);

            // Записать шаг
            var step = new OptimizationStep
            {
                Iteration = k,
                X = (double[])xStar.Clone(),
                FunctionValue = fValue,
                PenaltyValue = barrierValue,
                R = r
            };
            steps.Add(step);

            // Шаг 4: Проверка условия окончания
            if (Math.Abs(barrierValue) <= parameters.Epsilon)
            {
                result.OptimalPoint = xStar;
                result.OptimalValue = fValue;
                result.IterationCount = k + 1;
                return result;
            }

            // Обновить параметры для следующей итерации
            // В барьерном методе r уменьшается (r^{k+1} = r^k / C)
            r = r / parameters.BarrierDivisor;
            x = xStar;
            k++;
        }

        // Если достигнуто максимальное число итераций
        result.OptimalPoint = x;
        result.OptimalValue = _constrainedFunction.ObjectiveFunction(x);
        result.IterationCount = k;
        return result;
    }

    // Проверить, находится ли точка внутри допустимой области
    private bool IsInsideFeasibleRegion(double[] x)
    {
        double[] constraints = _constrainedFunction.InequalityConstraints(x);
        foreach (var g in constraints)
        {
            if (g >= 0) // Должно быть g < 0
                return false;
        }
        return true;
    }

    // Вычислить барьерную функцию P(x, r)
    private double CalculateBarrier(double[] x, double r, BarrierFunctionType barrierType)
    {
        double barrier = 0;
        double[] constraints = _constrainedFunction.InequalityConstraints(x);

        foreach (var g in constraints)
        {
            if (g >= 0)
            {
                // Если ограничение нарушено, вернуть большое значение
                return double.MaxValue;
            }

            switch (barrierType)
            {
                case BarrierFunctionType.Inverse:
                    // P(x, r) = -r * Σ(1/g_j(x))
                    barrier += 1.0 / g;
                    break;

                case BarrierFunctionType.Logarithmic:
                    // P(x, r) = -r * Σ ln(-g_j(x))
                    barrier += Math.Log(-g);
                    break;
            }
        }

        return -r * barrier;
    }

    // Вспомогательная функция F(x, r) = f(x) + P(x, r)
    private class AuxiliaryBarrierFunction : IFunction
    {
        private readonly IInequalityConstrainedFunction _constrainedFunc;
        private readonly double _r;
        private readonly BarrierFunctionType _barrierType;

        public AuxiliaryBarrierFunction(
            IInequalityConstrainedFunction constrainedFunc,
            double r,
            BarrierFunctionType barrierType)
        {
            _constrainedFunc = constrainedFunc;
            _r = r;
            _barrierType = barrierType;
        }

        public double Evaluate(double[] x)
        {
            double f = _constrainedFunc.ObjectiveFunction(x);
            double p = CalculateBarrier(x);

            // Если барьер бесконечен (нарушено ограничение), вернуть большое значение
            if (double.IsInfinity(p) || double.IsNaN(p))
                return double.MaxValue;

            return f + p;
        }

        public double[] Gradient(double[] x)
        {
            // Градиент вспомогательной функции = градиент f(x) + градиент P(x, r)
            double[] gradF = _constrainedFunc.ObjectiveFunctionGradient(x);
            double[] gradP = CalculateBarrierGradient(x);

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
            double[][] hessP = CalculateBarrierHessian(x);

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

        private double CalculateBarrier(double[] x)
        {
            double barrier = 0;
            double[] constraints = _constrainedFunc.InequalityConstraints(x);

            foreach (var g in constraints)
            {
                if (g >= 0)
                {
                    // Если ограничение нарушено
                    return double.PositiveInfinity;
                }

                switch (_barrierType)
                {
                    case BarrierFunctionType.Inverse:
                        barrier += 1.0 / g;
                        break;

                    case BarrierFunctionType.Logarithmic:
                        barrier += Math.Log(-g);
                        break;
                }
            }

            return -_r * barrier;
        }

        private double[] CalculateBarrierGradient(double[] x)
        {
            int n = x.Length;
            double[] grad = new double[n];

            // Численное дифференцирование
            double h = 1e-6;
            for (int i = 0; i < n; i++)
            {
                double[] xPlusH = (double[])x.Clone();
                xPlusH[i] += h;

                double[] xMinusH = (double[])x.Clone();
                xMinusH[i] -= h;

                double pPlus = CalculateBarrier(xPlusH);
                double pMinus = CalculateBarrier(xMinusH);

                if (double.IsInfinity(pPlus) || double.IsInfinity(pMinus))
                {
                    grad[i] = 0;
                }
                else
                {
                    grad[i] = (pPlus - pMinus) / (2 * h);
                }
            }

            return grad;
        }

        private double[][] CalculateBarrierHessian(double[] x)
        {
            int n = x.Length;
            double[][] hess = new double[n][];

            // Численное дифференцирование
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

                    double pPlusIJ = CalculateBarrier(xPlusIJ);
                    double pPlusI = CalculateBarrier(xPlusI);
                    double pPlusJ = CalculateBarrier(xPlusJ);
                    double p = CalculateBarrier(x);

                    if (double.IsInfinity(pPlusIJ) || double.IsInfinity(pPlusI) ||
                        double.IsInfinity(pPlusJ) || double.IsInfinity(p))
                    {
                        hess[i][j] = 0;
                    }
                    else
                    {
                        hess[i][j] = (pPlusIJ - pPlusI - pPlusJ + p) / (h * h);
                    }
                }
            }

            return hess;
        }
    }
}

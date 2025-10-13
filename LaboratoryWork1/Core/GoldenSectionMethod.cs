using LaboratoryWork1.Models;

namespace LaboratoryWork1.Core;

// Метод золотого сечения для поиска минимума функции
public class GoldenSectionMethod : IOptimizationMethod
{
    public string Name => "Метод золотого сечения";

    public OptimizationResult FindMinimum(IFunction function, double a, double b, double epsilon)
    {
        var result = new OptimizationResult();
        int k = 0;

        double ak = a;
        double bk = b;

        // Константа золотого сечения: (3 - √5)/2
        double goldenRatio = (3 - Math.Sqrt(5)) / 2;

        // Шаг 3: Вычислить начальные точки y и z
        double yk = ak + goldenRatio * (bk - ak);
        double zk = ak + bk - yk;

        while (true)
        {
            // Шаг 4: Вычислить значения функции
            double fYk = function.Calculate(yk);
            double fZk = function.Calculate(zk);

            // Шаг 5: Сравнить значения и исключить интервалы
            double akNext, bkNext, ykNext, zkNext;

            if (fYk <= fZk)
            {
                // Шаг 5а: Исключить интервал (zk, bk]
                akNext = ak;
                bkNext = zk;
                ykNext = akNext + bkNext - yk;
                zkNext = yk;
            }
            else
            {
                // Шаг 5б: Исключить интервал [ak, yk)
                akNext = yk;
                bkNext = bk;
                ykNext = zk;
                zkNext = akNext + bkNext - zk;
            }

            // Шаг 6: Вычислить delta следующего интервала
            double delta = Math.Abs(bkNext - akNext);

            // Сохранить шаг с delta следующего интервала
            result.Steps.Add(new OptimizationStep
            {
                Iteration = k,
                A = ak,
                B = bk,
                Xc = 0,
                Y = yk,
                Z = zk,
                FXc = 0,
                FY = fYk,
                FZ = fZk,
                IntervalLength = delta
            });

            if (delta <= epsilon)
            {
                result.OptimalX = (akNext + bkNext) / 2;
                result.OptimalValue = function.Calculate(result.OptimalX);
                result.FinalIntervalStart = akNext;
                result.FinalIntervalEnd = bkNext;
                break;
            }

            // Перейти к следующей итерации
            ak = akNext;
            bk = bkNext;
            yk = ykNext;
            zk = zkNext;
            k++;
        }

        return result;
    }
}

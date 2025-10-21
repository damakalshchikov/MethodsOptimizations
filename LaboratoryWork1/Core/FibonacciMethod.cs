using LaboratoryWork1.Models;

namespace LaboratoryWork1.Core;

// Метод чисел Фибоначчи для поиска минимума функции
public class FibonacciMethod : IOptimizationMethod
{
    public string Name => "Метод чисел Фибоначчи";

    public OptimizationResult FindMinimum(IFunction function, double a, double b, double l)
    {
        var result = new OptimizationResult();

        // Константа различимости
        double epsilon = l / 2;

        // Шаг 2: Найти N и вычислить числа Фибоначчи
        double initialLength = Math.Abs(b - a);
        int N = FindN(initialLength, l);
        long[] fibonacci = GenerateFibonacci(N);

        // Шаг 3: Положить k = 0
        int k = 0;

        double ak = a;
        double bk = b;

        // Шаг 4: Вычислить начальные точки y0 и z0
        double yk = ak + (double)fibonacci[N - 2] / fibonacci[N] * (bk - ak);
        double zk = ak + (double)fibonacci[N - 1] / fibonacci[N] * (bk - ak);

        while (true)
        {
            // Шаг 5: Вычислить значения функции
            double fYk = function.Calculate(yk);
            double fZk = function.Calculate(zk);

            // Вычислить delta текущего интервала
            double delta = Math.Abs(bk - ak);

            // Сохранить шаг
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

            // Шаг 6: Сравнить значения и исключить интервалы
            double akNext, bkNext, ykNext, zkNext;

            if (fYk <= fZk)
            {
                // Шаг 6а
                akNext = ak;
                bkNext = zk;
                zkNext = yk;

                if (k != N - 3)
                {
                    ykNext = akNext + (double)fibonacci[N - k - 3] / fibonacci[N - k - 1] * (bkNext - akNext);
                }
                else
                {
                    ykNext = yk; // Временное значение, будет пересчитано
                }
            }
            else
            {
                // Шаг 6б
                akNext = yk;
                bkNext = bk;
                ykNext = zk;

                if (k != N - 3)
                {
                    zkNext = akNext + (double)fibonacci[N - k - 2] / fibonacci[N - k - 1] * (bkNext - akNext);
                }
                else
                {
                    zkNext = zk; // Временное значение, будет пересчитано
                }
            }

            // Шаг 7: Проверить условие окончания
            if (k == N - 3)
            {
                // Заключительное вычисление
                double yFinal = (akNext + bkNext) / 2;
                double zFinal = yFinal + epsilon;

                double fYFinal = function.Calculate(yFinal);
                double fZFinal = function.Calculate(zFinal);

                double aFinal, bFinal;
                if (fYFinal <= fZFinal)
                {
                    aFinal = akNext;
                    bFinal = zFinal;
                }
                else
                {
                    aFinal = yFinal;
                    bFinal = bkNext;
                }

                result.OptimalX = (aFinal + bFinal) / 2;
                result.OptimalValue = function.Calculate(result.OptimalX);
                result.FinalIntervalStart = aFinal;
                result.FinalIntervalEnd = bFinal;
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

    // Найти наименьшее N, при котором F_N >= |L_0|/l
    private int FindN(double initialLength, double l)
    {
        double ratio = initialLength / l;
        int n = 0;
        long f0 = 1, f1 = 1;

        while (f1 < ratio)
        {
            long temp = f1;
            f1 = f0 + f1;
            f0 = temp;
            n++;
        }

        return n + 1;
    }

    // Генерировать числа Фибоначчи F_0, F_1, ..., F_N
    private long[] GenerateFibonacci(int N)
    {
        long[] fib = new long[N + 1];
        fib[0] = 1;
        if (N > 0) fib[1] = 1;

        for (int i = 2; i <= N; i++)
        {
            fib[i] = fib[i - 1] + fib[i - 2];
        }

        return fib;
    }
}

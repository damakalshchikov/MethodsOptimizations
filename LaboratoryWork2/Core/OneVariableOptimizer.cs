namespace LaboratoryWork2.Core;

// Класс для одномерной оптимизации (поиск минимума функции одной переменной)
public class OneVariableOptimizer
{
    // Метод золотого сечения для поиска минимума функции одной переменной
    // на интервале [a, b]
    public static double GoldenSection(Func<double, double> function, double a, double b, double epsilon = 1e-6)
    {
        double phi = (1 + Math.Sqrt(5)) / 2; // Золотое число
        double resphi = 2 - phi;

        // Начальные точки
        double x1 = a + resphi * (b - a);
        double x2 = b - resphi * (b - a);

        double f1 = function(x1);
        double f2 = function(x2);

        while (Math.Abs(b - a) > epsilon)
        {
            if (f1 < f2)
            {
                b = x2;
                x2 = x1;
                f2 = f1;
                x1 = a + resphi * (b - a);
                f1 = function(x1);
            }
            else
            {
                a = x1;
                x1 = x2;
                f1 = f2;
                x2 = b - resphi * (b - a);
                f2 = function(x2);
            }
        }

        return (a + b) / 2;
    }

    // Альтернативный метод: поиск минимума через производную (для гладких функций)
    // Использует метод половинного деления для поиска нуля производной
    public static double FindMinimumByDerivative(
        Func<double, double> function,
        Func<double, double> derivative,
        double a,
        double b,
        double epsilon = 1e-8)
    {
        // Используем метод Ньютона для поиска минимума
        // Минимум находится где производная = 0
        double x = (a + b) / 2;
        double h = 1e-6;

        for (int i = 0; i < 100; i++)
        {
            double f = derivative(x);

            if (Math.Abs(f) < epsilon)
                return x;

            // Численная вторая производная
            double f_prime = (derivative(x + h) - derivative(x - h)) / (2 * h);

            if (Math.Abs(f_prime) < 1e-12)
                break;

            // Шаг Ньютона
            double x_new = x - f / f_prime;

            // Если выходим за границы интервала, используем золотое сечение
            if (x_new < a || x_new > b)
            {
                return GoldenSection(function, a, b, epsilon);
            }

            if (Math.Abs(x_new - x) < epsilon)
                return x_new;

            x = x_new;
        }

        return x;
    }

    // Практический метод поиска минимума для случаев, когда неизвестна производная
    public static double Minimize(Func<double, double> function, double tMax = 1.0, double epsilon = 1e-8)
    {
        // Используем метод золотого сечения на интервале [0, tMax]
        return GoldenSection(function, 0.0, tMax, epsilon);
    }

    // Перегрузка для работы с методом минимизации
    public static double MinimizeLineSearch(Func<double, double> phi, double maxT = 1.0)
    {
        return GoldenSection(phi, 0.0, maxT, 1e-8);
    }
}

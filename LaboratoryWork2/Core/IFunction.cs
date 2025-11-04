namespace LaboratoryWork2.Core;

// Интерфейс для функции многих переменных
public interface IFunction
{
    // Вычислить значение функции в точке x
    double Evaluate(double[] x);

    // Вычислить градиент функции в точке x
    double[] Gradient(double[] x);
}

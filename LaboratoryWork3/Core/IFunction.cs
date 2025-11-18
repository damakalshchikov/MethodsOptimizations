namespace LaboratoryWork3.Core;

// Интерфейс для функции многи переменных
public interface IFunction
{
    // Вычислить значение функции в точке x
    double Evaluate(double[] x);

    // Вычислить градиент функции в точке x
    double[] Gradient(double[] x);

    // Вычислить матрицу Гессе (матрицу вторых производных) в точке x
    double[][] Hessian(double[] x);
}

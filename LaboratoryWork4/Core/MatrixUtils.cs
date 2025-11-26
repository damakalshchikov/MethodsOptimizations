namespace LaboratoryWork4.Core;

// Утилиты для работы с матрицами и векторами
public class MatrixUtils
{
    // Инвертировать квадратную матрицу 2x2
    public static double[][] Invert2x2(double[][] matrix)
    {
        if (matrix.Length != 2 || matrix[0].Length != 2 || matrix[1].Length != 2)
            throw new ArgumentException("Матрица должна быть 2x2");

        double a = matrix[0][0];
        double b = matrix[0][1];
        double c = matrix[1][0];
        double d = matrix[1][1];

        double det = a * d - b * c;

        if (Math.Abs(det) < 1e-12)
            throw new ArgumentException("Матрица сингулярна (определитель равен нулю)");

        return new[]
        {
            new[] { d / det, -b / det },
            new[] { -c / det, a / det }
        };
    }

    // Умножить матрицу на вектор: result = matrix * vector
    public static double[] MatrixVectorMultiply(double[][] matrix, double[] vector)
    {
        int n = matrix.Length;
        double[] result = new double[n];

        for (int i = 0; i < n; i++)
        {
            result[i] = 0;
            for (int j = 0; j < vector.Length; j++)
            {
                result[i] += matrix[i][j] * vector[j];
            }
        }

        return result;
    }

    // Проверить, является ли матрица положительно определенной
    public static bool IsPositiveDefinite(double[][] matrix)
    {
        int n = matrix.Length;

        // Для 2x2 матрицы: A > 0, D > 0, A*D - B*C > 0
        if (n == 2)
        {
            return matrix[0][0] > 0 &&
                   matrix[1][1] > 0 &&
                   (matrix[0][0] * matrix[1][1] - matrix[0][1] * matrix[1][0]) > 0;
        }

        return false;
    }

    // Вычислить норму вектора
    public static double VectorNorm(double[] vector)
    {
        double sum = 0;
        foreach (var v in vector)
        {
            sum += v * v;
        }
        return Math.Sqrt(sum);
    }
}

namespace LaboratoryWork3.Core;

// Утилиты для работы с матрицами
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

    // Инвертировать квадратную матрицу методом Гаусса-Жордана
    public static double[][] Invert(double[][] matrix)
    {
        int n = matrix.Length;

        // Создать расширенную матрицу [A | I]
        double[][] extended = new double[n][];
        for (int i = 0; i < n; i++)
        {
            extended[i] = new double[2 * n];
            for (int j = 0; j < n; j++)
            {
                extended[i][j] = matrix[i][j];
            }
            extended[i][n + i] = 1;
        }

        // Прямой ход
        for (int i = 0; i < n; i++)
        {
            // Найти ведущий элемент
            int maxRow = i;
            for (int k = i + 1; k < n; k++)
            {
                if (Math.Abs(extended[k][i]) > Math.Abs(extended[maxRow][i]))
                    maxRow = k;
            }

            // Переставить строки
            (extended[i], extended[maxRow]) = (extended[maxRow], extended[i]);

            // Проверить на сингулярность
            if (Math.Abs(extended[i][i]) < 1e-12)
                throw new ArgumentException("Матрица сингулярна");

            // Нормализовать строку i
            double pivot = extended[i][i];
            for (int j = 0; j < 2 * n; j++)
            {
                extended[i][j] /= pivot;
            }

            // Исключить столбец i в других строках
            for (int k = 0; k < n; k++)
            {
                if (k != i)
                {
                    double factor = extended[k][i];
                    for (int j = 0; j < 2 * n; j++)
                    {
                        extended[k][j] -= factor * extended[i][j];
                    }
                }
            }
        }

        // Извлечь обратную матрицу из правой части
        double[][] result = new double[n][];
        for (int i = 0; i < n; i++)
        {
            result[i] = new double[n];
            for (int j = 0; j < n; j++)
            {
                result[i][j] = extended[i][n + j];
            }
        }

        return result;
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

        // Для большей матрицы используем критерий Сильвестра
        for (int k = 1; k <= n; k++)
        {
            double[][] minor = new double[k][];
            for (int i = 0; i < k; i++)
            {
                minor[i] = new double[k];
                for (int j = 0; j < k; j++)
                {
                    minor[i][j] = matrix[i][j];
                }
            }

            double det = Determinant(minor);
            if (det <= 0)
                return false;
        }

        return true;
    }

    // Вычислить определитель матрицы
    public static double Determinant(double[][] matrix)
    {
        int n = matrix.Length;

        if (n == 1)
            return matrix[0][0];

        if (n == 2)
            return matrix[0][0] * matrix[1][1] - matrix[0][1] * matrix[1][0];

        double det = 0;
        for (int j = 0; j < n; j++)
        {
            det += (j % 2 == 0 ? 1 : -1) * matrix[0][j] * Determinant(GetMinor(matrix, 0, j));
        }

        return det;
    }

    // Получить минор матрицы (вычеркнуть строку и столбец)
    private static double[][] GetMinor(double[][] matrix, int row, int col)
    {
        int n = matrix.Length;
        double[][] minor = new double[n - 1][];

        int minorRow = 0;
        for (int i = 0; i < n; i++)
        {
            if (i == row)
                continue;

            minor[minorRow] = new double[n - 1];
            int minorCol = 0;
            for (int j = 0; j < n; j++)
            {
                if (j == col)
                    continue;

                minor[minorRow][minorCol] = matrix[i][j];
                minorCol++;
            }
            minorRow++;
        }

        return minor;
    }

    // Скопировать матрицу
    public static double[][] Copy(double[][] matrix)
    {
        double[][] result = new double[matrix.Length][];
        for (int i = 0; i < matrix.Length; i++)
        {
            result[i] = new double[matrix[i].Length];
            Array.Copy(matrix[i], result[i], matrix[i].Length);
        }
        return result;
    }
}

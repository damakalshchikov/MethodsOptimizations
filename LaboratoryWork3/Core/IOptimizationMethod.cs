using LaboratoryWork3.Models;

namespace LaboratoryWork3.Core;

// Интерфейс для методов оптимизации
public interface IOptimizationMethod
{
    // Выполнить оптимизацию функции
    OptimizationResult Optimize(IFunction function, double[] initialPoint, OptimizationParameters parameters);
}

// Параметры для оптимизации
public class OptimizationParameters
{
    // Первый критерий окончания - норма градиента
    public double Epsilon1 { get; set; }

    // Второй критерий окончания - изменение точки и функции
    public double Epsilon2 { get; set; }

    // Максимальное число итераций
    public int MaxIterations { get; set; }

    // Начальный шаг (используется, если Гессе не положительно определена)
    public double InitialStepSize { get; set; } = 0.5;

    // Параметр для сильного убывания функции
    public double StrongDescentParameter { get; set; } = 0.1;
}

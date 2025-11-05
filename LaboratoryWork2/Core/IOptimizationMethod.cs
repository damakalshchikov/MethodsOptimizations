using LaboratoryWork2.Models;

namespace LaboratoryWork2.Core;

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

    // Начальный шаг
    public double InitialStepSize { get; set; } = 0.5;

    // Параметр для сильного убывания функции (альтернативное условие в шаге 8)
    public double StrongDescentParameter { get; set; } = 0.1;
}

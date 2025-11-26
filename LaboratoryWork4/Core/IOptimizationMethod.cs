using LaboratoryWork4.Models;

namespace LaboratoryWork4.Core;

// Интерфейс для методов оптимизации
public interface IOptimizationMethod
{
    // Выполнить оптимизацию функции
    OptimizationResult Optimize(IFunction function, double[] initialPoint, OptimizationParameters parameters);
}

// Параметры для оптимизации
public class OptimizationParameters
{
    // Точность остановки для градиента
    public double Epsilon1 { get; set; }

    // Точность остановки для изменения точки
    public double Epsilon2 { get; set; }

    // Максимальное число итераций
    public int MaxIterations { get; set; }

    // Начальный шаг для поиска в направлении
    public double InitialStepSize { get; set; } = 0.5;
}

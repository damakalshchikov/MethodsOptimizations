using LaboratoryWork1.Core;

namespace LaboratoryWork1.Tasks.Task2;

// f(x) = 2x³ + 9x² - 21
public class Task2Function : IFunction
{
    public string Name => "f(x) = 2x³ + 9x² - 21";

    public double Calculate(double x)
    {
        return 2 * Math.Pow(x, 3) + 9 * Math.Pow(x, 2) - 21;
    }
}

using LaboratoryWork2.Tasks.Task1;
using LaboratoryWork2.Tasks.Task2;

Console.WriteLine("Выберите задачу:");
Console.WriteLine("1 - Метод градиентного спуска с постоянным шагом");
Console.WriteLine("2 - Метод наискорейшего градиентного спуска");
Console.Write("Введите номер (1 или 2): ");

string? choice = Console.ReadLine();

if (choice == "2")
{
    var solver2 = new Task2Solver();
    var result2 = solver2.Solve();
    solver2.PrintResult(result2);
}
else
{
    var solver1 = new Task1Solver();
    var result1 = solver1.Solve();
    solver1.PrintResult(result1);
}

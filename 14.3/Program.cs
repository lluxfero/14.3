using System.Diagnostics;
using System.Runtime.CompilerServices;

Console.Write("введите число, определяющее кол-во потоков и кол-во итераций: ");
Test.N = Convert.ToInt32(Console.ReadLine());

List<Task> tasks = new();
Stopwatch time = new();

for (int i = 0; i < Test.N; i++)
{
    Task task = new(Test.GetPi);
    tasks.Add(task);
}

time.Start();
for (int i = 0; i < Test.N; i++) tasks[i].Start();
for (int i = 0; i < Test.N; i++) tasks[i].Wait();
time.Stop();
Console.WriteLine($"\n == результат == " +
                  $"\n  количество потоков: {Test.N}" +
                  $"\n  количество итераций: {Test.N}" +
                  $"\n  затраченное время: {time.ElapsedTicks} ticks" +
                  $"\n  значение pi: {string.Format("{0:0.00}", Test.Pi * 4)}"); // ряд Лейбница получает pi/4

class Test
{
    public static double Pi { get; set; } = 1;
    public static AutoResetEvent Waiter { get; set; } = new(true);
    public static int N { get; set; } = 0;
    public static int X { get; set; } = 0;

    public static void GetPi()
    {
        Waiter.WaitOne();
        while (X < N)
        {
            X++;
            if (X % 2 != 0) Pi -= 1.0 / (X * 2 + 1);
            else Pi += 1.0 / (X * 2 + 1);
        }
        Waiter.Set();
    }
}
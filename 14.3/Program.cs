using System.Diagnostics;
using System.Runtime.CompilerServices;

Console.Write("введите число, определяющее кол-во потоков: ");
int threads = Convert.ToInt32(Console.ReadLine());
Console.Write("введите число, определяющее кол-во итераций: ");
Test.Iter = Convert.ToInt32(Console.ReadLine());

List<Task> tasks = new();
Stopwatch time = new();

for (int i = 0; i < threads; i++)
{
    Task task = new(Test.GetPi);
    tasks.Add(task);
}

time.Start();
for (int i = 0; i < threads; i++) tasks[i].Start();
for (int i = 0; i < threads; i++) tasks[i].Wait();
time.Stop();
Console.WriteLine($"\n == результат == " +
                  $"\n  количество потоков: {threads}" +
                  $"\n  количество итераций: {Test.Iter}" +
                  $"\n  затраченное время: {time.ElapsedTicks} ticks" +
                  $"\n  значение pi: {string.Format("{0:0.00}", Test.Pi * 4)}"); // ряд Лейбница получает pi/4

class Test
{
    public static double Pi { get; set; } = 1;
    public static AutoResetEvent Waiter { get; set; } = new(true);
    public static int Iter { get; set; } = 0;
    public static int X { get; set; } = 0;

    public static void GetPi()
    {
        
        while (X < Iter)
        {
            Waiter.WaitOne();
            X++;
            if (X % 2 != 0) Pi -= 1.0 / (X * 2 + 1);
            else Pi += 1.0 / (X * 2 + 1);
            Waiter.Set();
        }
        
    }
}
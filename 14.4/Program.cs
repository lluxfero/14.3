using System.Diagnostics;
using System.Security.Cryptography;

Random rnd = new((int)DateTime.Now.Ticks);

int[] arr = new int[30];
for (int i = 0; i < arr.Length; i++)
    arr[i] = rnd.Next(0, 100);

for (int i = 0; i < arr.Length; i++)
{
    Console.Write($"{arr[i]} ");
    if (i % 10 == 0 && i != 0) Console.WriteLine();
}
Console.WriteLine("\n");

AutoResetEvent waiter = new(true);
int n = 0;
Stopwatch time = Stopwatch.StartNew();
var select = arr.AsParallel()
                .Select(Increment) // арифметическая функция для каждого элемента
                .GroupBy(x => x % 10) // группировка по кратности элемента десяти
                .Select(x => x.Sum()) // сумма групп
                .OrderBy(x => x) // сортировка
                .ToArray();
                /*.ForAll(x =>
                {
                    waiter.WaitOne();
                    if (n >= 5 && n <= 7) Console.Write($"{x} "); // вывод
                    n++;
                    waiter.Set();
                });*/
for (int i = 4; i < 7; i++) Console.Write($"{select[i]} "); // вывод
time.Stop();
Console.WriteLine($"\n\tвремя: {time.ElapsedTicks} ticks\n");


for (int i = 1; i < 17; i += 3)
{
    n = 0;
    time.Restart();
    var select0 = arr.AsParallel()
                .WithDegreeOfParallelism(i) // использует не более одного процессора, если i == 1
                .Select(Increment)
                .GroupBy(x => x % 10)
                .Select(x => x.Sum())
                .OrderBy(x => x) // сортировка
                .ToArray();
                /*.ForAll(x =>
                {
                    waiter.WaitOne();
                    if (n >= 5 && n <= 7) 
                        Console.Write($"{x} "); // вывод
                    n++;
                    waiter.Set();
                });*/
    for (int j = 4; j < 7; j++) Console.Write($"{select0[j]} "); // вывод
    time.Stop();
    Console.WriteLine($"\n\tвремя (степень паралелизма {i}) {time.ElapsedTicks} ticks\n");
}

static int Increment(int x)
{
    return ++x;
}

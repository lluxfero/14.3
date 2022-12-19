using System.Diagnostics;
using System.Security.Cryptography;

Random rnd = new((int)DateTime.Now.Ticks);
int n = 0;

int[] arr = new int[10];
for (int i = 0; i < arr.Length; i++)
    arr[i] = rnd.Next(0, 100);

for (int i = 0; i < arr.Length; i++)
{
    Console.Write($"{arr[i]} ");
    if (i % 10 == 0 && i != 0) Console.WriteLine();
}
Console.WriteLine("\n");

Stopwatch time = Stopwatch.StartNew();
arr.AsParallel()
                .Select(Increment) // арифметическая функция для каждого элемента
                .GroupBy(x => x % 10) // группировка по кратности элемента десяти
                .Select(x => x.Sum()) // сумма групп
                .OrderBy(x => x) // сортировка
                .ForAll(x =>
                {
                    if (n >= 5 && n <= 7) Console.Write($"{x} "); // вывод
                    n++;
                });
time.Stop();
Console.WriteLine($"\n\tвремя: {time.ElapsedTicks} ticks\n");

#region experiments
n = 0;
time.Restart();
arr.AsParallel().WithDegreeOfParallelism(1) // использует не более одного процессора
                .Select(Increment)
                .GroupBy(x => x % 10)
                .Select(x => x.Sum())
                .OrderBy(x => x)
                .ForAll(x =>
                {
                    if (n >= 5 && n <= 7) Console.Write($"{x} "); // вывод
                    n++;
                });
time.Stop();
Console.WriteLine($"\n\tвремя (степень паралелизма 1): {time.ElapsedTicks} ticks\n");

n = 0;
time.Restart();
arr.AsParallel().WithDegreeOfParallelism(3)
                .Select(Increment)
                .GroupBy(x => x % 10)
                .Select(x => x.Sum())
                .OrderBy(x => x)
                .ForAll(x =>
                {
                    if (n >= 5 && n <= 7) Console.Write($"{x} "); // вывод
                    n++;
                });
time.Stop();
Console.WriteLine($"\n\tвремя (степень паралелизма 3): {time.ElapsedTicks} ticks\n");

n = 0;
time.Restart();
arr.AsParallel().WithDegreeOfParallelism(5)
                .Select(Increment)
                .GroupBy(x => x % 10)
                .Select(x => x.Sum())
                .OrderBy(x => x)
                .ForAll(x =>
                {
                    if (n >= 5 && n <= 7) Console.Write($"{x} "); // вывод
                    n++;
                });
time.Stop();
Console.WriteLine($"\n\tвремя (степень паралелизма 5): {time.ElapsedTicks} ticks\n");

n = 0;
time.Restart();
arr.AsParallel().WithDegreeOfParallelism(10)
                .Select(Increment)
                .GroupBy(x => x % 10)
                .Select(x => x.Sum())
                .OrderBy(x => x)
                .ForAll(x =>
                {
                    if (n >= 5 && n <= 7) Console.Write($"{x} "); // вывод
                    n++;
                });
time.Stop();
Console.WriteLine($"\n\tвремя (степень паралелизма 10): {time.ElapsedTicks} ticks\n");

n = 0;
time.Restart();
arr.AsParallel().WithDegreeOfParallelism(15)
                .Select(Increment)
                .GroupBy(x => x % 10)
                .Select(x => x.Sum())
                .OrderBy(x => x)
                .ForAll(x =>
                {
                    if (n >= 5 && n <= 7) Console.Write($"{x} "); // вывод
                    n++;
                });
time.Stop();
Console.WriteLine($"\n\tвремя (степень паралелизма 15): {time.ElapsedTicks} ticks\n");
#endregion

static int Increment(int x)
{
    return ++x;
}

int[] arr = new int[1000];
for (int i = 0; i < 1000; ++i)
{
    arr[i] = Random.Shared.Next(0, 100000);
}

List<int>[] hundreds = new List<int>[10];
for (int i = 0; i < 10; ++i)
{
    hundreds[i] = new();
}
for (int i = 0; i < 1000; ++i)
{
    hundreds[arr[i] / 100 % 10].Add(arr[i]);
}

Parallel.ForEach(hundreds,
    new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
(list, state, index) =>
{
    using (var file = new StreamWriter(index + ".txt"))
    {
        foreach (var item in list)
        {
            file.WriteLine(item);
        }
    }
});
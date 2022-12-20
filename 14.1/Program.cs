using System.Diagnostics;

Stopwatch sw = new Stopwatch();

int count = 10;
TimeSpan last_span = TimeSpan.Zero;
for (int i = 1; i <= count; i++)
{
    Console.WriteLine($"Creating {i} threads");

    // Для того, чтобы точно определить, что все потоки запустились
    // используем CountdownEvent
    using (var countdownEvent = new CountdownEvent(i))
    {
        sw.Restart();
        for (int j = 0; j < i; j++)
        {
            new Thread(delegate ()
            {
                countdownEvent.Signal();
            }).Start();
        }
        countdownEvent.Wait();
        Console.WriteLine($"Started {i} threads in {last_span.Ticks} ticks\n");
    }

    Thread.Sleep(100);

    Console.WriteLine($"Creating {i} threads in threadpool");

    using (var countdownEvent = new CountdownEvent(i))
    {
        sw.Restart();
        for (int j = 0; j < i; ++j)
        {
            ThreadPool.QueueUserWorkItem(delegate (Object? obj)
            {
                countdownEvent.Signal();
            });
        }

        countdownEvent.Wait();
        last_span = sw.Elapsed;
        Console.WriteLine($"Started threadpool {i} threads in {last_span.Ticks} ticks\n");
    }

}
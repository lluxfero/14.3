var countdownGeneral = new CountdownEvent(3); // блок для трёх главных результатов
var countdownB = new CountdownEvent(2); // блок для операций В
var countdownCStreakD = new CountdownEvent(3); // 
EventWaitHandle BWaiter = new AutoResetEvent(false);
EventWaitHandle CWaiter = new AutoResetEvent(false);
EventWaitHandle CStreakWaiter = new AutoResetEvent(false);

Random rnd = new((int)DateTime.Now.Ticks);


string b = await ResultBAsync();
await ResultCAsync();
string a = await ResultAAsync();

Console.WriteLine(a);
Console.WriteLine(b);



async Task<string> ResultAAsync()
{
    string a = OperationA();
    BWaiter.Set(); // пропускаем результат В
    string b = OperationB("ResultA");
    string c = OperationC();

    countdownGeneral.Signal();
    countdownGeneral.Wait();

    return $"{a} -> {b} -> {c}";
}

async Task<string> ResultBAsync()
{
    BWaiter.WaitOne(); // ждем пока закончится операция А результата А
    string b = String.Empty;
    string astreak = String.Empty;
    await Task.Run(() => astreak = OperationAStreak());
    await Task.Run(() => b = OperationB("ResultB"));

    CWaiter.Set();

    string d = String.Empty;
    await Task.Run(() => d = OperationD());
    string cstreak = OperationCStreak();

    countdownGeneral.Signal();
    countdownGeneral.Wait();

    return $"{astreak} -> {b} -> {d} -> {cstreak}";
}

async Task ResultCAsync()
{
    CWaiter.WaitOne(); // ждем сигнала из результата В
    Console.WriteLine($"Result C started in {DateTime.Now}");

    await Task.Delay(rnd.Next(1000, 10000));

    countdownCStreakD.Signal();
    countdownCStreakD.Wait();

    await Task.Delay(rnd.Next(1000, 10000));
    Console.WriteLine($"Result C completed in {DateTime.Now}");
    countdownGeneral.Signal();
    countdownGeneral.Wait();
    return;
}

string OperationA()
{
    Console.WriteLine($"Operation A started in {DateTime.Now}");
    Thread.Sleep(rnd.Next(1000, 10000));
    Console.WriteLine($"Operation A completed in {DateTime.Now}");
    return "A";
}

string OperationB(string source)
{
    Console.WriteLine($"Operation B started from {source} in {DateTime.Now}");
    Thread.Sleep(rnd.Next(1000, 10000));
    countdownB.Signal(); // ждем другую операцию В
    countdownB.Wait();
    Console.WriteLine($"Operation B completed in {source} in {DateTime.Now}");
    return "B";
}

string OperationC()
{
    Console.WriteLine($"Operation C started in {DateTime.Now}");
    Thread.Sleep(rnd.Next(1000, 10000));
    Console.WriteLine($"Operation C completed in {DateTime.Now}");
    CStreakWaiter.Set();
    return "C";
}

string OperationD()
{
    Console.WriteLine($"Operation D started in {DateTime.Now}");
    Thread.Sleep(rnd.Next(1000, 10000));
    countdownCStreakD.Signal();
    countdownCStreakD.Wait();
    Console.WriteLine($"Operation D completed in {DateTime.Now}");
    return "D";
}

string OperationAStreak()
{
    Console.WriteLine($"Operation A' started in {DateTime.Now}");
    Thread.Sleep(rnd.Next(1000, 10000));
    Console.WriteLine($"Operation A' completed in {DateTime.Now}");
    return "A'";
}

string OperationCStreak()
{
    CStreakWaiter.WaitOne();

    Console.WriteLine($"Operation C' started in {DateTime.Now}");
    Thread.Sleep(rnd.Next(1000, 10000));

    countdownCStreakD.Signal();
    countdownCStreakD.Wait();

    Console.WriteLine($"Operation C' completed in {DateTime.Now}");
    return "C'";
}
// Создаём линии
// Для управлением буфером, меняем 3ий параметр на [1..inf(?)]
// в зависимости от того, хотим ли мы копить или нет
ProductionLine ALine = new("A", 800, 5, new());
ProductionLine BLine = new("B", 1500, 5, new());

ProductionLine VLine = new("V", 3300, 5, new());
ProductionLine MLine = new("M", 2500, 5, new List<ProductionLine> { ALine, BLine });
ProductionLine NLine = new("N", 2500, 5, new List<ProductionLine> { VLine, MLine });

ALine.Run();
BLine.Run();
VLine.Run();
MLine.Run();
NLine.ThrashBuffer = true;
NLine.Run();

const int delay = 10000;
int step = 1;
while (step < 4)
{
    Thread.Sleep(delay);
    Console.WriteLine($"\t\t\t\t{delay * step / 1000} seconds elapsed");
    Console.WriteLine($"\t\t\t\tA line produced : {ALine.TotalProduced}");
    Console.WriteLine($"\t\t\t\tB line produced : {BLine.TotalProduced}");
    Console.WriteLine($"\t\t\t\tV line produced : {VLine.TotalProduced}");
    Console.WriteLine($"\t\t\t\tM line produced : {MLine.TotalProduced}");
    Console.WriteLine($"\t\t\t\tN line produced : {NLine.TotalProduced}");
    ++step;
}


public class ProductionLine
{
    public string Name;
    public int Delay = 0;
    public int MaxBuffer = 1;
    public bool ThrashBuffer = false;
    public int Buffer
    {
        get
        {
            // Перед тем, как попытаться получить значение, ждём мутекс
            _buf_mutex.WaitOne();
            _buf_mutex.ReleaseMutex();
            return _buffer;
        }
        set
        {
            _buf_mutex.WaitOne();
            _buffer = value;

            // Если буфер 0, то продукта в запасе(и в целом) нет, 
            // значит те, кто от нас зависит, должен ждать пока мы что-то сделаем
            if (_buffer <= 0)
            {
                _buffer = 0;
                IsProductAvailable.Reset();
            }

            // Если мы сделали наш план, то прекращаем производить
            if (_buffer >= MaxBuffer)
            {
                Console.WriteLine($"\t{Name} can`t produce!");
                IsProducable.Reset();
            }
            else
            {
                Console.WriteLine($"\t{Name} can produce");
                IsProducable.Set();
            }

            _buf_mutex.ReleaseMutex();
        }
    }
    private Mutex _buf_mutex = new Mutex();
    private int _buffer = 0;

    public AutoResetEvent IsProductAvailable = new AutoResetEvent(false);
    public AutoResetEvent IsProducable = new AutoResetEvent(true);
    public List<ProductionLine> Dependencies;

    public int TotalProduced
    {
        get
        {
            _total_mut.WaitOne();
            _total_mut.ReleaseMutex();
            return _totalprod;
        }
        private set
        {
            _total_mut.WaitOne();
            _totalprod = value;
            _total_mut.ReleaseMutex();
        }
    }
    private int _totalprod;
    private Mutex _total_mut = new Mutex();
    public ProductionLine(string name, int delay, int maxBuffer, List<ProductionLine> dependencies)
    {
        Name = name;
        Delay = delay;
        MaxBuffer = maxBuffer;
        Dependencies = dependencies;
    }

    public void Run()
    {
        Task.Run(() => {
            while (true)
            {
                foreach (var dep in Dependencies)
                {
                    // Ждём, пока в буффере у нашей зависимости что-то будет
                    Console.WriteLine($"\tfrom {Name} : waiting for {dep.Name}");
                    dep.IsProductAvailable.WaitOne();
                    Console.WriteLine($"\tfrom {Name} : {dep.Name} is available!");
                    --dep.Buffer;
                }
                Console.WriteLine($"\tfrom {Name} : dep ok!");
                IsProducable.WaitOne();
                Console.WriteLine($"\tfrom {Name} : prod ok!");
                Thread.Sleep(Delay);
                ++Buffer;
                if (ThrashBuffer)
                {
                    Buffer = 0;
                }
                ++TotalProduced;
                // Так как мы что-то произвели, даём сигнал зависимостям, что мы готовы отдавать
                IsProductAvailable.Set();
                Console.WriteLine($"Product {Name} is made!");
            }
        });
    }
}
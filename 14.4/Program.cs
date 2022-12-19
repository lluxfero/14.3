Random rnd = new Random((int)DateTime.Now.Ticks);

int[] arr = new int[100];
for (int i = 0; i < arr.Length; i++)
    arr[i] = rnd.Next(-100, 100);



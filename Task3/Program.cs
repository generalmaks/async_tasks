namespace Task3;

class Program
{
    static void Main(string[] args)
    {
        Func<string, CancellationToken, Task<string>> func = (item, token) =>
        {
            Task<string> result = Task.Run(() =>
            {
                if (item.Length > 10)
                {
                    var result = item.Substring(0, 10);
                    Console.WriteLine($"Result: {result}");
                    return result;
                }
                if(token.IsCancellationRequested)
                    throw new Exception("Cancellation Token requested");
                throw new Exception($"Bad item: {item}");
            });
            return result;
        };
        var strings = new[]
        {
            "Hello World!",
            "Goodbye World!",
            "See you again!",
            "Goodbye friend!",
            "Hello again!",
            "See you tomorrow!",
        };
        
        var cts = new CancellationTokenSource();
        Console.WriteLine("Operation has started. Press any key to abort");
        var finalTask = MapAsync(strings, func, cts.Token);
        
        Task.Run(() =>
        {
            Console.ReadKey();
            cts.Cancel();
            Console.WriteLine("ABORTED");
        });

        finalTask.ContinueWith(result =>
        {
            if (result.IsFaulted || result.IsCanceled)
            {
                var a = result.Exception.Flatten().InnerExceptions;
                foreach (var ex in a)
                    Console.WriteLine(ex.GetBaseException().Message);
            }
        }).Wait();

        Console.WriteLine("Operation has ended. Press any key to end");
        Console.ReadKey();
    }

    static Task<T[]> MapAsync<T>(T[] array, Func<T, CancellationToken, Task<string>> func, CancellationToken token = default)
    {
        var t = Task.Run(() =>
        {
            int length = array.Length;
            T[] results = new T[length];
            Task[] tasks = new Task[length];

            for (int i = 0; i < length; i++)
            {
                Task.Delay(1000).Wait();
                token.ThrowIfCancellationRequested();

                int index = i;
                tasks[index] = func(array[index], token);
            }

            Task.WaitAll(tasks, token);
            return results;
        }, token);
        return t;
    }
}
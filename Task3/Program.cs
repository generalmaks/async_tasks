namespace Task3;

class Program
{
    static void Main(string[] args)
    {
        Func<string, Task<string>> func = (item) =>
        {
            Task<string> result = Task.Run(() =>
            {
                if (item.Length > 10)
                    return item.Substring(0, 10);
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
        try
        {
            finalTask.Wait();
        }
        catch (AggregateException ae)
        {
            Console.WriteLine("Errors occurred:");
            foreach (var inner in ae.InnerExceptions)
                Console.WriteLine(inner.Message);
        }

        Console.WriteLine("Operation has ended. Press any key to end");
        Console.ReadKey();
    }

    static Task<T[]> MapAsync<T>(T[] array, Func<T, Task<T>> func, CancellationToken token)
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
                tasks[index] = func(array[index]).ContinueWith(task =>
                {
                    Console.WriteLine("IS FAULTED");
                    if (task.IsFaulted)
                        throw task.Exception.GetBaseException();
                    else if (task.IsCanceled)
                        Console.WriteLine($"Task canceled at index {index}");
                    else
                        Console.WriteLine($"$RESULT: {task.Result}");
                    results[index] = task.Result;
                }, token);
            }

            Task.WaitAll(tasks);
            return results;
        }, token);
        return t;
    }
}
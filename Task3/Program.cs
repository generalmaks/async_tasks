namespace Task2;

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
        var strings = new string[]
        {
            "Hello World!",
            "Goodbye World!",
            "See you again!",
            "Goodbye friend!",
            "Hello again!",
            "See you tomorrow!",
        };
        var finalTask = MapAsync(strings, func);

        finalTask.ContinueWith(result =>
        {
            if (result.IsFaulted)
            {
                var a = result.Exception.Flatten().InnerExceptions;
                foreach (var ex in a)
                    Console.WriteLine(ex.GetBaseException().Message);
            }
            else
                foreach (var item in result.Result)
                {
                    Console.WriteLine($"Result: {item}");
                }
        }).Wait();

        Func<int, Task<int>> func2 = (number) =>
        {
            Task<int> result = Task.Run(() =>
            {
                int delayNumber = number * 100;
                Task.Delay(delayNumber);
                if (number % 2 == 0)
                    return number * 2;
                else
                    return number;
            });
            return result;
        };

        var numbers = new int[25];
        for (int i = 0; i < numbers.Length; i++)
            numbers[i] = i;
        var finalTask2 = MapAsync(numbers, func2);
        finalTask2.ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Console.WriteLine($"Task failed: {task.Exception?.GetBaseException().Message}");
            }
            else
            {
                // Output the results
                foreach (var result in task.Result)
                {
                    Console.WriteLine($"Result: {result}");
                }
            }
        });
        Console.ReadLine();
    }

    static Task<T[]> MapAsync<T>(T[] array, Func<T, Task<T>> func)
    {
        var t = Task.Run(() =>
        {
            int length = array.Length;
            T[] results = new T[length];
            Task[] tasks = new Task[length];

            for (int i = 0; i < length; i++)
            {
                int index = i;
                tasks[index] = func(array[index]).ContinueWith(task =>
                {
                    if (task.IsFaulted)
                        throw task.Exception.GetBaseException();

                    results[index] = task.Result;
                });
            }

            Task.WaitAll(tasks);
            return results;
        });
        return t;
    }
}
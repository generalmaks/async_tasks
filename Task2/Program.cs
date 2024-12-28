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
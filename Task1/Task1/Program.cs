namespace Task1;

class Program
{
    static void Main(string[] args)
    {
    }


    static void MapAsync<T>(T[] array, Action<T, Action<ItemInfo<T>>> asyncCallback,
        Action<Exception[], T[]> callbackFinal)
    {
        int arrayLength = array.Length;

        List<T> results = new List<T>();
        List<Exception> errors = new List<Exception>();

        int counter = 0;

        object locker = new object();

        foreach (T item in array)
        {
            asyncCallback(item, (cortege) =>
            {
                lock (locker)
                {
                    if (cortege.Error is not null)
                    {
                        errors.Add(cortege.Error);
                    }
                    else
                    {
                        results.Add(cortege.Value);
                    }

                    counter++;

                    if (counter == arrayLength)
                        callbackFinal(errors.ToArray(), results.ToArray());
                }
            });
        }
    }
}

public struct ItemInfo<T>
{
    public ItemInfo(Exception error)
    {
        Error = error;
        Value = default;
    }

    public ItemInfo(T value)
    {
        Error = null;
        Value = value;
    }

    public Exception Error { get; }
    public T Value { get; }
}
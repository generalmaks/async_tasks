namespace Task4;

class Program
{
    static async Task Main(string[] args)
    {
        string filePath = "largefile.txt";
        int size = 500;
        await foreach (var line in ReadLines(filePath, size))
        {
            Console.WriteLine(line);
        }
    }

    async static IAsyncEnumerable<char[]> ReadLines(string filePath, int size)
    {
        using var reader = new StreamReader(filePath);
        while (!reader.EndOfStream)
        {
            await Task.Delay(500);
            char[] buffer = new char[size];
            await reader.ReadAsync(buffer, 0, size);
            yield return buffer;
        }
    }
}
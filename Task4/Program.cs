namespace Task4;

class Program
{
    static async Task Main(string[] args)
    {
        string filePath = "largefile.txt";
        await foreach (var line in ReadLines(filePath))
        {
            Console.WriteLine(line);
        }
    }

    async static IAsyncEnumerable<string> ReadLines(string filePath)
    {
        using var reader = new StreamReader(filePath);
        while (!reader.EndOfStream)
        {
            yield return reader.ReadLine();
        }
    }
}
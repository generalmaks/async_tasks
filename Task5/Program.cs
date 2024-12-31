using System;
using System.Reactive.Subjects;

class Program
{
    static void Main()
    {
        var subject = new Subject<string>();

        var subscription1 = subject.Subscribe(
            message => Console.WriteLine($"Subscriber 1 received: {message}"),
            ex => Console.WriteLine($"Subscriber 1 encountered an error: {ex.Message}"),
            () => Console.WriteLine("Subscriber 1 completed")
        );

        var subscription2 = subject.Subscribe(
            message => Console.WriteLine($"Subscriber 2 received: {message}"),
            ex => Console.WriteLine($"Subscriber 2 encountered an error: {ex.Message}"),
            () => Console.WriteLine("Subscriber 2 completed")
        );

        subject.OnNext("Hello, Reactive Extensions!");
        subject.OnNext("Learning Rx in C#");

        subject.OnCompleted();

        subscription1.Dispose();
        subscription2.Dispose();

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}

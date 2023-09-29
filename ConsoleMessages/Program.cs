namespace ConsoleMessages
{
    internal class Program
    {
        async static Task Main(string[] args)
        {            
            PrintThreadToConsoleAsync(1000);
            PrintThreadToConsoleAsync(2000);
            PrintThreadToConsoleAsync(3000);
            await PrintLineAsync(1001);
        }

        async static Task PrintLineAsync(int sleepTime)
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    Console.WriteLine();
                    Thread.Sleep(sleepTime);                    
                }
            });

        }
        async static Task PrintThreadToConsoleAsync(int sleepTime)
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(sleepTime);
                    Console.WriteLine($"Это поток {Task.CurrentId}, выполняемый с задержкой {sleepTime}");                    
                }
            });
            
        }
    }
}
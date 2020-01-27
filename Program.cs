namespace MilkshakeCup
{
    using System;
    using System.IO;
    using Microsoft.Extensions.Configuration;

    public class Program
    {
        public static void Main(string[] args)
        {
            var bot = CreateBotClient();
            bot.StartReceiving();

            Console.WriteLine($"MilkshakeCup Bot up and running!");
            Console.WriteLine("Press esc to exit");

            for (var key = Console.ReadKey(); !IsEsc(key); key = Console.ReadKey()) ;

            bot.StopReceiving();
        }

        private static MilkshakeCupTelegramBotClient CreateBotClient() =>
            new MilkshakeCupTelegramBotClient(
                Configuration()["token"],
                new CSVFileGroupsRepository("Groups"));

        private static IConfigurationRoot Configuration() =>
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json")
                .Build();

        private static bool IsEsc(ConsoleKeyInfo keyInfo) => keyInfo.Key == ConsoleKey.Escape;
    }
}
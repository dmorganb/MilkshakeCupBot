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

            while(Console.ReadKey().Key != ConsoleKey.Escape) ;

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
    }
}
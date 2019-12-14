namespace MilkshakeCup
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Telegram.Bot;
    using MilkshakeCup.Models;

    public class Program
    {
        private static ITelegramBotClient botClient;

        private static IGroupsRepository groupsRepository { get; set; }

        public static async Task Main(string[] args)
        {
            // app settings
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true);
            var configuration = builder.Build();
            var token = configuration["token"];
            var spreadsheetId = configuration["spreadsheetId"];

            groupsRepository = new CSVFileGroupsRepository("Groups");

            // bot client
            botClient = new MilkshakeCupTelegramBotClient(token, groupsRepository);
            botClient.StartReceiving();

            // intro
            var me = await botClient.GetMeAsync();
            Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");
            Console.WriteLine("Press esc to exit");

            ConsoleKeyInfo input;

            do
            {
                input = Console.ReadKey();
            }
            while (input.Key != ConsoleKey.Escape);
        }
   }
}
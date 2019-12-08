namespace MilkshakeCup
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Telegram.Bot;
    using Telegram.Bot.Args;
    using Telegram.Bot.Types.Enums;
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

            // bot client
            botClient = new TelegramBotClient(token);
            botClient.OnMessage += OnMessage;
            botClient.StartReceiving();

            groupsRepository = new CSVFileGroupsRepository("Groups");
            
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

        private static async void OnMessage(object sender, MessageEventArgs e) 
        {
            try
            {
                Console.WriteLine(e.Message.Text);
                
                if (e.Message.Text == null)
                {
                    return;
                }

                if (e.Message.Text.StartsWith("/tabla"))
                {
                    await TablasCommand(e);
                    return;
                }
                if (e.Message.Text.StartsWith("/marcador"))
                {
                    await MarcadorCommand(e);
                    return;
                }
            }
            catch(Exception ex)
            {
                // fail silently 
                Console.WriteLine($"There was an exception with message e: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        private static async Task TablasCommand(MessageEventArgs e)
        {
            var parameters = e.Message.Text.Split(' ');
            var selectedGroups = "abc";

            if (parameters.Length > 1)
            {
                selectedGroups = parameters[1]?.ToLower();
            }

            selectedGroups = (selectedGroups == "a" || selectedGroups == "b" || selectedGroups == "c") ? selectedGroups : "abc";

            foreach (var group in groupsRepository.Groups())
            {
                if (selectedGroups.Contains(group.Name))
                {
                    var message = await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: GroupTableMessage(group),
                        parseMode: ParseMode.Markdown,
                        disableNotification: true,
                        replyToMessageId: e.Message.MessageId);
                }
            }
        }

        private static async Task MarcadorCommand(MessageEventArgs e)
        {
            var parameters = e.Message.Text.Split(' ');

            if (parameters.Length != 5)
            {
                var message = await botClient.SendTextMessageAsync(
                    chatId: e.Message.Chat,
                    text: "Marcador debe llevar: equipo goles equipo goles\nPor ejemplo: Atletico 1 Eibar 0",
                    parseMode: ParseMode.Default,
                    disableNotification: true,
                    replyToMessageId: e.Message.MessageId);
            }
            else
            {
                var team1 = parameters[1]?.ToLower();
                var goals1 = -1;
                var team2 = parameters[3]?.ToLower();
                var goals2 = -1;

                if (!int.TryParse(parameters[2], out goals1))
                {
                    goals1 = -1;
                }
                
                if(!int.TryParse(parameters[4], out goals2))
                {
                    goals2 = -1;
                }

                var groups = groupsRepository.Groups();
                var row1 = groups.Select(x => x.Row(team1)).FirstOrDefault(x => x != null);
                var row2 = groups.Select(x => x.Row(team2)).FirstOrDefault(x => x != null);

                // row validations:

                // 1) teams or players exist in a group.
                if (row1 == null || row2 == null)
                {
                    var message = await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: $"no encontré alguno de estos equipos: {team1}, {team2}",
                        parseMode: ParseMode.Default,
                        disableNotification: true,
                        replyToMessageId: e.Message.MessageId);
                    return;
                }

                // 2) teams or players are different.
                if (row1 == row2)
                {
                    var message = await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: $"Diay, jugó solo?",
                        parseMode: ParseMode.Default,
                        disableNotification: true,
                        replyToMessageId: e.Message.MessageId);
                    return;
                }

                var row1Group = groups.FirstOrDefault(x => x.Has(row1));
                var row2Group = groups.FirstOrDefault(x => x.Has(row2));

                // 3) Teams belong to the same group.
                if ((row1Group.Has(row1) && !row1Group.Has(row2)) ||
                    (row2Group.Has(row1) && !row2Group.Has(row2)))
                {
                    var message = await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: $"Buen intento, pero solo acepto partidos de equipos del mismo grupo :P",
                        parseMode: ParseMode.Default,
                        disableNotification: true,
                        replyToMessageId: e.Message.MessageId);
                    return;
                }

                // score validations

                // 1) goals reported are valid scores
                if (goals1 < 0 || goals2 < 0)
                {
                    var message = await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: $"Alguno de estos marcadores esta mamando: {parameters[2]}, {parameters[4]}",
                        parseMode: ParseMode.Default,
                        disableNotification: true,
                        replyToMessageId: e.Message.MessageId);
                    return;
                }

                const int goalsThreshold = 10; // if someone scored more than 10 in the same game, that's suspicious.
                
                // 2) no one scored more than 10 goals (possible but not probable)
                if (goals1 > goalsThreshold || goals2 > goalsThreshold)
                {
                    var message = await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: $"Mejor vuelvan a jugarlo, pero esta vez con todos los controles encendidos",
                        parseMode: ParseMode.Default,
                        disableNotification: true,
                        replyToMessageId: e.Message.MessageId);
                    return;
                }

                // all parameters have valid values
                if (row1 != null && row2 != null && goals1 >= 0 && goals2 >= 0)
                {
                    if (goals1 > goals2) // team 1 won, team 2 lost
                    {
                        row1.Won++;
                        row2.Lost++;
                    }
                    else if (goals1 < goals2) // team 1 lost, team 2 won
                    {
                        row1.Lost++;
                        row2.Won++;
                    }
                    else // team 1 & team 2 draw
                    {
                        row1.Draw++;
                        row2.Draw++;
                    }

                    // goals bookkeeping
                    row1.GoalsInFavor += goals1;
                    row1.GoalsAgainst += goals2;
                    row2.GoalsInFavor += goals2;
                    row2.GoalsAgainst += goals1;

                    Console.WriteLine(row1Group == row2Group);

                    // save current table to Drive
                    groupsRepository.Save(row1Group); // row1Group and row2GroupShouldBeTheSame

                    // confirmation message
                    var message = await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: $"Anotado!",
                        parseMode: ParseMode.Default,
                        disableNotification: true,
                        replyToMessageId: e.Message.MessageId);
                }

            }
        }

        private static string GroupTableMessage(Group group) 
        {
            var text = "Group " + group.Name + "\n";
            text += "```\n";
            text += "Equ|Pt|PJ| G| E| P|GF|Dif\n";
            text += "=========================\n";

            var orderedGroup = group.Rows;

            foreach (var row in orderedGroup)
            {
                text += (row.Player.Length > 3? row.Player.Substring(0, 3) : row.Player.PadRight(3)) + "|";
                text += $"{row.Points}".PadLeft(2) + "|";
                text += $"{row.TotalGames}".PadLeft(2) + "|";
                text += $"{row.Won}".PadLeft(2) + "|";
                text += $"{row.Draw}".PadLeft(2) + "|";
                text += $"{row.Lost}".PadLeft(2) + "|";
                text += $"{row.GoalsInFavor}".PadLeft(2) + "|";
                text += $"{row.GoalDifference}".PadLeft(3) + "\n";
            }

            text += "```";

            return text;
        }
    }
}

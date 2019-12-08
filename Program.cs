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
            catch (Exception ex)
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
                var player1Hint = parameters[1]?.ToLower();
                var player1Goals = -1;
                
                if (!int.TryParse(parameters[2], out player1Goals))
                {
                    player1Goals = -1;
                }

                var player2Hint = parameters[3]?.ToLower();
                var player2Goals = -1;

                if (!int.TryParse(parameters[4], out player2Goals))
                {
                    player2Goals = -1;
                }

                // score validations

                // 1) goals reported are valid scores
                if (player1Goals < 0 || player2Goals < 0)
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
                if (player1Goals > goalsThreshold || player2Goals > goalsThreshold)
                {
                    var message = await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: $"Mejor vuelvan a jugarlo, pero esta vez con todos los controles encendidos",
                        parseMode: ParseMode.Default,
                        disableNotification: true,
                        replyToMessageId: e.Message.MessageId);
                    return;
                }

                var groups = groupsRepository.Groups();
                var player1 = groups.Select(x => x.PlayerByHint(player1Hint)).FirstOrDefault(x => x != null);
                var player2 = groups.Select(x => x.PlayerByHint(player2Hint)).FirstOrDefault(x => x != null);

                // Player validations:

                // 1) teams or players exist.
                if (player1 == null || player2 == null)
                {
                    var message = await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: $"no encontré alguno de estos equipos: {player1Hint}, {player2Hint}",
                        parseMode: ParseMode.Default,
                        disableNotification: true,
                        replyToMessageId: e.Message.MessageId);
                    return;
                }

                // 2) Players are different.
                if (player1 == player2)
                {
                    var message = await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: $"Diay, jugó solo?",
                        parseMode: ParseMode.Default,
                        disableNotification: true,
                        replyToMessageId: e.Message.MessageId);
                    return;
                }

                // 3) Players belong to the same group.
                if (player1.Group != player2.Group)
                {
                    var message = await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: $"Buen intento, pero solo acepto partidos de equipos del mismo grupo :P",
                        parseMode: ParseMode.Default,
                        disableNotification: true,
                        replyToMessageId: e.Message.MessageId);
                    return;
                }

                // Everything is correct at this point, go ahead and update the group.

                if (player1Goals > player2Goals) // player 1 won, player 2 lost
                {
                    player1.Wins(player1Goals, player2Goals);
                    player2.Loses(player2Goals, player1Goals);
                }
                else if (player1Goals < player2Goals) // player 1 lost, player 2 won
                {
                    player1.Loses(player1Goals, player2Goals);
                    player2.Wins(player2Goals, player1Goals);
                }
                else // player 1 & player 2 draw
                {
                    player1.Draws(player1Goals, player2Goals);
                    player2.Draws(player2Goals, player1Goals);
                }

                // saves the group (player1.Group should be the same as player2.Group at this point)
                groupsRepository.Save(player1.Group);

                // confirmation message
                var confirmationMessage = await botClient.SendTextMessageAsync(
                    chatId: e.Message.Chat,
                    text: $"Anotado!",
                    parseMode: ParseMode.Default,
                    disableNotification: true,
                    replyToMessageId: e.Message.MessageId);
            }
        }

        private static string GroupTableMessage(Group group)
        {
            var text = "Group " + group.Name + "\n";
            text += "```\n";
            text += "Equ|Pt|PJ| G| E| P|GF|Dif\n";
            text += "=========================\n";

            foreach (var player in group.Players)
            {
                text += (player.Name.Length > 3 ? player.Name.Substring(0, 3) 
                            : player.Name.PadRight(3)) + "|";
                text += $"{player.Points}".PadLeft(2) + "|";
                text += $"{player.TotalGames}".PadLeft(2) + "|";
                text += $"{player.Won}".PadLeft(2) + "|";
                text += $"{player.Draw}".PadLeft(2) + "|";
                text += $"{player.Lost}".PadLeft(2) + "|";
                text += $"{player.GoalsInFavor}".PadLeft(2) + "|";
                text += $"{player.GoalDifference}".PadLeft(3) + "\n";
            }

            text += "```";

            return text;
        }
    }
}
﻿namespace MilkshakeCup
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Telegram.Bot;
    using Telegram.Bot.Args;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;
    using Telegram.Bot.Types.ReplyMarkups;

    public class Program
    {
        private static ITelegramBotClient botClient;
        
        private static List<Row> GroupA { get; set; }

        private static List<Row> GroupB { get; set; }

        private class Row{

            public string Player { get; set; }
            
            public string Team { get; set; }
            
            public int Points => (Won * 3) + Draw;

            public int TotalGames => Won + Lost + Draw;

            public int Won { get; set; }

            public int Draw { get; set; }

            public int Lost { get; set; }            
            
            public int GoalsInFavor { get; set; }

            public int GoalsAgainst { get; set; }

            public int GoalDifference => GoalsInFavor - GoalsAgainst;

            public Row()
            {
            }

            public Row(string player, string team, int won, int draw, int lost, int goalsInFavor, int goalsAgainst){
                Player = player;
                Team = team;
                Won = won;
                Draw = draw;
                Lost = lost;
                GoalsInFavor = goalsInFavor;
                GoalsAgainst = goalsAgainst;
            }
        }

        public static async Task Main(string[] args)
        {
            // app settings
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            var token = configuration["token"];

            var groupAFile = await System.IO.File.ReadAllLinesAsync("GroupA.csv");
            GroupA = new List<Row>();
            foreach(var line in groupAFile)
            {
                var fields = line.Split(',');
                GroupA.Add(new Row(
                    fields[0],
                    fields[1],
                    int.Parse(fields[2]),
                    int.Parse(fields[3]),
                    int.Parse(fields[4]),
                    int.Parse(fields[5]),
                    int.Parse(fields[6])));
            }

            var groupBFile = await System.IO.File.ReadAllLinesAsync("GroupB.csv");
            GroupB = new List<Row>();
            foreach(var line in groupBFile)
            {
                var fields = line.Split(',');
                GroupB.Add(new Row(
                    fields[0],
                    fields[1],
                    int.Parse(fields[2]),
                    int.Parse(fields[3]),
                    int.Parse(fields[4]),
                    int.Parse(fields[5]),
                    int.Parse(fields[6])));
            }

            // bot client
            botClient = new TelegramBotClient(token);
            botClient.OnMessage += Bot_OnMessage;
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
            while(input.Key != ConsoleKey.Escape);
        }

        static async void Bot_OnMessage(object sender, MessageEventArgs e) 
        {
            try
            {
            Console.WriteLine(e.Message.Text);
            if (e.Message.Text == "/tablas")
            {
                var text = GroupTableMessage("Grupo A", GroupA);
                Console.WriteLine($"Received a text message in chat {e.Message.Chat.Id}.");

                var message1 = await botClient.SendTextMessageAsync(
                    chatId: e.Message.Chat,
                    text: text,
                    parseMode: ParseMode.Markdown,
                    disableNotification: true,
                    replyToMessageId: e.Message.MessageId
                );

                var message2 = await botClient.SendTextMessageAsync(
                    chatId: e.Message.Chat,
                    text: GroupTableMessage("Grupo B", GroupB),
                    parseMode: ParseMode.Markdown,
                    disableNotification: true,
                    replyToMessageId: e.Message.MessageId
                );
            }
            else if(e.Message.Text != null && e.Message.Text.StartsWith("/marcador"))
            {
                var parameters = e.Message.Text.Split(' ');
                if(parameters.Length != 5){
                    var message = await botClient.SendTextMessageAsync(
                    chatId: e.Message.Chat,
                    text: "Marcador debe llevar: equipo goles equipo goles\nPor ejemplo: Atletico 1 Eibar 0",
                    parseMode: ParseMode.Markdown,
                    disableNotification: true,
                    replyToMessageId: e.Message.MessageId
                    );
                }
                else 
                {
                    var team1 = parameters[1];
                    int goals1 = -1;
                    int.TryParse(parameters[2], out goals1);
                    var team2 = parameters[3];
                    var goals2 = -1;
                    int.TryParse(parameters[4], out goals2);


                    var row1 = GroupA.FirstOrDefault(x => x.Team.StartsWith(team1) || x.Player.Contains(team1)) ?? 
                               GroupB.FirstOrDefault(x => x.Team.StartsWith(team1) || x.Player.Contains(team1));
                    var row2 = GroupA.FirstOrDefault(x => x.Team.StartsWith(team2) || x.Player.Contains(team2)) ?? 
                               GroupB.FirstOrDefault(x => x.Team.StartsWith(team2) || x.Player.Contains(team2));

                    if(row1 == null || row2 == null)
                    {
                        var message = await botClient.SendTextMessageAsync(
                            chatId: e.Message.Chat,
                            text: $"no encontré alguno de estos equipos: {team1}, {team2}",
                            parseMode: ParseMode.Markdown,
                            disableNotification: true,
                            replyToMessageId: e.Message.MessageId);
                    }

                    if(goals1 < 0 || goals2 < 0)
                    {
                        var message = await botClient.SendTextMessageAsync(
                            chatId: e.Message.Chat,
                            text: $"Alguno de estos marcadores esta mamando: {parameters[2]}, {parameters[4]}",
                            parseMode: ParseMode.Markdown,
                            disableNotification: true,
                            replyToMessageId: e.Message.MessageId);
                    }

                    // todo bien, todo correcto
                    if(row1 != null && row2 != null && goals1 >= 0 && goals2 >= 0)
                    {
                            
                        // ganó team 1.
                        if(goals1 > goals2)
                        {
                            row1.Won++;
                            row2.Lost++;
                        }
                        else if(goals1 < goals2)
                        {
                            row1.Lost++;
                            row2.Won++;
                        }
                        else
                        {
                            row1.Draw++;
                            row2.Draw++;
                        }

                        row1.GoalsInFavor += goals1;
                        row1.GoalsAgainst += goals2;
                        row2.GoalsInFavor += goals2;
                        row2.GoalsAgainst += goals1;

                        await System.IO.File.WriteAllLinesAsync("GroupA.csv", Csv(GroupA));
                        await System.IO.File.WriteAllLinesAsync("GroupB.csv", Csv(GroupB));

                        var message = await botClient.SendTextMessageAsync(
                            chatId: e.Message.Chat,
                            text: $"Anotado!",
                            parseMode: ParseMode.Markdown,
                            disableNotification: true,
                            replyToMessageId: e.Message.MessageId);
                    }

                }
            }
            }
            catch(Exception ex)
            {
                // fail silently 
                Console.WriteLine($"There was an exception with exception e: {ex.Message}");
            }
        }

        private static List<string> Csv(List<Row> group)
        {
            var lines = new List<string>();
            
            foreach(var row in group)
            {
                lines.Add($"{row.Player},{row.Team},{row.Won},{row.Draw},{row.Lost},{row.GoalsInFavor},{row.GoalsAgainst}");
            }

            return lines;
        }

        private static string GroupTableMessage(string title, List<Row> group) 
        {
            var text = title + "\n";
            
            text += "```\n";
            text += "Equipo|Pts| PJ| G| E| P| GF|GC|Dif\n";
            text += "------+---+---+--+--+--+---+--+---\n";

            foreach(var row in group.OrderByDescending(x => x.Points))
            {
                text += (row.Player.Length > 6? row.Player.Substring(0, 6) : row.Player.PadRight(6)) + "|";
                text += $" {row.Points.ToString("00")}|";
                text += $" {row.TotalGames.ToString("00")}|";
                text += $"{row.Won.ToString("00")}|";
                text += $"{row.Draw.ToString("00")}|";
                text += $"{row.Lost.ToString("00")}|";
                text += $" {row.GoalsInFavor.ToString("00")}|";
                text += $"{row.GoalsAgainst.ToString("00")}|";
                text += $"{row.GoalDifference.ToString("00")}".PadLeft(3) + "\n";
            }

            text += "```";

            return text;
        }
    }
}
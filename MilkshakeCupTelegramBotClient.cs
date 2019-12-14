namespace MilkshakeCup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MilkshakeCup.Commands;
    using MilkshakeCup.Models;
    using Telegram.Bot;
    using Telegram.Bot.Args;
    using CommandInfo = System.Collections.Generic.KeyValuePair<string, Commands.Command>;

    public class MilkshakeCupTelegramBotClient : TelegramBotClient
    {
        private static readonly Dictionary<string, Command> _commands =
            new Dictionary<string, Command>
            {
                { "/tablas", GroupsCommand.Execute },
                { "/tabla", SingleGroupCommand.Execute },
                { "/marcador", MatchCommand.Execute },
                { "/borrar", RevertCommand.Execute }
            };

        private static readonly CommandInfo _notFound = default(CommandInfo);

        private readonly IGroupsRepository _groupsRepository;

        public MilkshakeCupTelegramBotClient(string token, IGroupsRepository groupsRepository)
            : base(token)
        {
            _groupsRepository = groupsRepository;
            OnMessage += HandleCommand;
        }

        private async void HandleCommand(object sender, MessageEventArgs e)
        {
            try
            {
                if (!CommandInfo(e).Equals(_notFound))
                {
                    var command = CommandInfo(e).Value;
                    await command(CommandContext(sender, e));
                }
            }
            catch (Exception ex)
            {
                // fail silently 
                Console.WriteLine($"There was an exception with message e: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        private static CommandInfo CommandInfo(MessageEventArgs e) =>
             _commands.FirstOrDefault(commandInfo => Text(e).StartsWith(commandInfo.Key));

        private static string Text(MessageEventArgs e) => e?.Message?.Text ?? "";

        private CommandContext CommandContext(object sender, MessageEventArgs e) =>
            new CommandContext(
                _groupsRepository,
                this,
                e,
                sender,
                Text(e).Split(' '));
    }
}
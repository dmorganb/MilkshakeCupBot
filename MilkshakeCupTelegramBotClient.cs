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

        private async void HandleCommand(object sender, MessageEventArgs eventArgs)
        {
            try
            {
                if (!CommandInfo(eventArgs).Equals(_notFound))
                {
                    await CommandInfo(eventArgs).Value(CommandContext(sender, eventArgs));
                }
            }
            catch (Exception ex)
            {
                // fail silently 
                Console.WriteLine($"There was an exception with message e: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        private static CommandInfo CommandInfo(MessageEventArgs eventArgs) =>
             _commands.FirstOrDefault(commandInfo => Text(eventArgs).StartsWith(commandInfo.Key));

        private static string Text(MessageEventArgs e) => e?.Message?.Text ?? "";

        private CommandContext CommandContext(object sender, MessageEventArgs eventArgs) =>
            new CommandContext(
                _groupsRepository,
                this,
                eventArgs,
                sender,
                Text(eventArgs).Split(' '));
    }
}
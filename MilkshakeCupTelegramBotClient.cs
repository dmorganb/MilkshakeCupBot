namespace MilkshakeCup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MilkshakeCup.Commands;
    using MilkshakeCup.Models;
    using Telegram.Bot;
    using Telegram.Bot.Args;

    public class MilkshakeCupTelegramBotClient : TelegramBotClient
    {
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
                if (e?.Message?.Text == null)
                {
                    return;
                }

                Console.WriteLine(e.Message.Text);

                var selectedCommandInfo = AvailableCommands().FirstOrDefault(
                    commandInfo => e.Message.Text.StartsWith(commandInfo.Key));

                if (!selectedCommandInfo.Equals(NotFound()))
                {
                    await selectedCommandInfo.Value(Context(sender, e));
                }

            }
            catch (Exception ex)
            {
                // fail silently 
                Console.WriteLine($"There was an exception with message e: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        private static Dictionary<string, Func<MilkshakeCupCommandContext, Task>> AvailableCommands() =>
            new Dictionary<string, Func<MilkshakeCupCommandContext, Task>>
            {
                { "/tablas", GroupsCommand.Execute },
                { "/tabla", SingleGroupCommand.Execute },
                { "/marcador", MatchCommand.Execute },
                { "/borrar", RevertCommand.Execute }
            };

        private static KeyValuePair<string, Func<MilkshakeCupCommandContext, Task>> NotFound() =>
            default(KeyValuePair<string, Func<MilkshakeCupCommandContext, Task>>);

        private MilkshakeCupCommandContext Context(object sender, MessageEventArgs e) =>
            new MilkshakeCupCommandContext(
                _groupsRepository,
                this,
                e,
                sender,
                e.Message.Text.Split(' '));
    }
}
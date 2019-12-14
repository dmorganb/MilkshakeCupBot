namespace MilkshakeCup
{
    using System;
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

                Func<MilkshakeCupCommandContext, Task> command;

                if (e.Message.Text.StartsWith("/tablas"))
                {
                    command = GroupsCommand.Execute;
                }
                else if (e.Message.Text.StartsWith("/tabla"))
                {
                    command = SingleGroupCommand.Execute;                    
                }
                else if (e.Message.Text.StartsWith("/marcador"))
                {
                    command = MatchCommand.Execute;
                }
                else if (e.Message.Text.StartsWith("/borrar"))
                {
                    command = RevertCommand.Execute;
                }
                else
                {
                    command = NotFoundCommand.Execute;
                }

                await command(new MilkshakeCupCommandContext(
                    _groupsRepository,
                    this,
                    e,
                    sender,
                    e.Message.Text.Split(' ')));
            }
            catch (Exception ex)
            {
                // fail silently 
                Console.WriteLine($"There was an exception with message e: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
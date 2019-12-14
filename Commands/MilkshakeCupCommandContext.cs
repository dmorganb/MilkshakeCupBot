namespace MilkshakeCup.Commands
{
    using MilkshakeCup.Models;
    using Telegram.Bot;
    using Telegram.Bot.Args;

    public class MilkshakeCupCommandContext
    {
        public IGroupsRepository GroupsRepository { get; private set; }

        public ITelegramBotClient TelegramBotClient  { get; private set; }

        public MessageEventArgs MessageEventArgs { get; private set; }

        public object Sender { get; set; }

        public string[] Parameters { get; private set; }

        public MilkshakeCupCommandContext(
            IGroupsRepository groupsRepository,
            ITelegramBotClient telegramBotClient,
            MessageEventArgs messageEventArgs,
            object sender,
            string[] parameters)
        {
            GroupsRepository = groupsRepository;
            TelegramBotClient = telegramBotClient;
            MessageEventArgs = messageEventArgs;
            Sender = sender;
            Parameters = parameters;
        }
    }    
}
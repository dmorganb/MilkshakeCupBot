namespace MilkshakeCup.Commands
{
    using System.Threading.Tasks;
    using MilkshakeCup.Models;
    using Telegram.Bot;
    using Telegram.Bot.Args;

    public delegate Task Command(CommandContext context);

    public class CommandContext
    {
        public IGroupsRepository GroupsRepository { get; private set; }

        public ITelegramBotClient TelegramBotClient { get; private set; }

        public MessageEventArgs MessageEventArgs { get; private set; }

        public object Sender { get; private set; }

        public string[] Parameters { get; private set; }

        public CommandContext(
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
namespace MilkshakeCup.Commands
{
    using System.Threading.Tasks;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public static class CommandContextExtensions
    {
        public static async Task<Message> SendMessage(
            this CommandContext context,
            string text)
        {
            var message = await context.TelegramBotClient.SendTextMessageAsync(
                chatId: context.MessageEventArgs.Message.Chat,
                text: text,
                parseMode: ParseMode.Default,
                disableNotification: true,
                replyToMessageId: context.MessageEventArgs.Message.MessageId);

            return message;
        }

        public static async Task<Message> SendMarkdownMessage(
            this CommandContext context,
            string markdownText)
        {
            var message = await context.TelegramBotClient.SendTextMessageAsync(
                chatId: context.MessageEventArgs.Message.Chat,
                text: markdownText,
                parseMode: ParseMode.Markdown,
                disableNotification: true,
                replyToMessageId: context.MessageEventArgs.Message.MessageId);

            return message;
        }
    }
}
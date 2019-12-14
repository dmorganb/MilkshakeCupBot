namespace MilkshakeCup.Commands
{
    using System.Linq;
    using System.Threading.Tasks;
    using Telegram.Bot.Types.Enums;

    public static class MatchCommand
    {
        // TODO a lot of these validations are "business logic" and should be in the "domain model"
        // (maybe a Group.Match(Player, Player, Score)) or something like that . . . 
        public static async Task Execute(MilkshakeCupCommandContext context)
        {
            if (context.Parameters.Length != 5)
            {
                var message = await context.TelegramBotClient.SendTextMessageAsync(
                    chatId: context.MessageEventArgs.Message.Chat,
                    text: "Marcador debe llevar: equipo goles equipo goles\nPor ejemplo: Atletico 1 Eibar 0",
                    parseMode: ParseMode.Default,
                    disableNotification: true,
                    replyToMessageId: context.MessageEventArgs.Message.MessageId);

                return;
            }

            var player1Hint = context.Parameters[1]?.ToLower();
            var player1Goals = -1;
            var player2Hint = context.Parameters[3]?.ToLower();
            var player2Goals = -1;

            var playersGroup = context.GroupsRepository.Groups().FirstOrDefault(
                group =>
                    group.PlayerByHint(player1Hint) != null &&
                    group.PlayerByHint(player2Hint) != null);

            if (playersGroup == null)
            {
                var message = await context.TelegramBotClient.SendTextMessageAsync(
                    chatId: context.MessageEventArgs.Message.Chat,
                    text: $"no encontré un grupo con: {player1Hint}, {player2Hint}",
                    parseMode: ParseMode.Default,
                    disableNotification: true,
                    replyToMessageId: context.MessageEventArgs.Message.MessageId);

                return;
            }

            // score validations            
            if (!int.TryParse(context.Parameters[2], out player1Goals))
            {
                player1Goals = -1;
            }

            if (!int.TryParse(context.Parameters[4], out player2Goals))
            {
                player2Goals = -1;
            }

            // 1) goals reported are valid scores
            if (player1Goals < 0 || player2Goals < 0)
            {
                var message = await context.TelegramBotClient.SendTextMessageAsync(
                    chatId: context.MessageEventArgs.Message.Chat,
                    text: $"Alguno de estos marcadores esta mamando: {context.Parameters[2]}, {context.Parameters[4]}",
                    parseMode: ParseMode.Default,
                    disableNotification: true,
                    replyToMessageId: context.MessageEventArgs.Message.MessageId);

                return;
            }

            const int goalsThreshold = 10; // if someone scored more than 10 in the same game, that's suspicious.

            // 2) no one should score more than 10 goals (possible but not probable)
            if (player1Goals > goalsThreshold || player2Goals > goalsThreshold)
            {
                var message = await context.TelegramBotClient.SendTextMessageAsync(
                    chatId: context.MessageEventArgs.Message.Chat,
                    text: $"Mejor vuelvan a jugarlo, pero esta vez con todos los controles encendidos",
                    parseMode: ParseMode.Default,
                    disableNotification: true,
                    replyToMessageId: context.MessageEventArgs.Message.MessageId);

                return;
            }

            var player1 = playersGroup.PlayerByHint(player1Hint);
            var player2 = playersGroup.PlayerByHint(player2Hint);

            // 2) Players must be different.
            if (player1 == player2)
            {
                var message = await context.TelegramBotClient.SendTextMessageAsync(
                    chatId: context.MessageEventArgs.Message.Chat,
                    text: $"Diay, jugó solo?",
                    parseMode: ParseMode.Default,
                    disableNotification: true,
                    replyToMessageId: context.MessageEventArgs.Message.MessageId);

                return;
            }

            // Everything is correct at this point, go ahead and update the group.
            player1.Match(player1Goals, player2Goals);
            player2.Match(player2Goals, player1Goals);
            context.GroupsRepository.Save(playersGroup);

            // confirmation message
            var confirmationMessage = await context.TelegramBotClient.SendTextMessageAsync(
                chatId: context.MessageEventArgs.Message.Chat,
                text: $"Anotado!",
                parseMode: ParseMode.Default,
                disableNotification: true,
                replyToMessageId: context.MessageEventArgs.Message.MessageId);
        }
    }
}
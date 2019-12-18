namespace MilkshakeCup.Commands
{
    using System.Linq;
    using System.Threading.Tasks;

    public static class MatchCommand
    {
        // TODO a lot of these validations are "business logic" and should be in the "domain model"
        // (maybe a Group.Match(Player, Player, Score)) or something like that . . . 
        public static async Task Execute(CommandContext context)
        {
            if (context.Parameters.Length != 5)
            {
                await context.SendMessage("Marcador debe llevar: equipo goles equipo goles\nPor ejemplo: Atletico 1 Eibar 0");
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
                await context.SendMessage($"no encontré un grupo con: {player1Hint}, {player2Hint}");
                return;
            }

            // score validations    
            // 1) goals reported are valid scores        
            if (!int.TryParse(context.Parameters[2], out player1Goals))
            {
                await context.SendMessage($"Este marcador esta mamando: {context.Parameters[2]}");
                return;
            }

            if (!int.TryParse(context.Parameters[4], out player2Goals))
            {
                await context.SendMessage($"Este marcador esta mamando: {context.Parameters[4]}");
                return;
            }

            // if someone scored more than 10 in the same game, that's suspicious.
            const int goalsThreshold = 10; 

            // 2) no one should score more than 10 goals (possible but not probable)
            if (player1Goals > goalsThreshold || player2Goals > goalsThreshold)
            {
                await context.SendMessage("Mejor vuelvan a jugarlo, pero esta vez con todos los controles encendidos");
                return;
            }

            var player1 = playersGroup.PlayerByHint(player1Hint);
            var player2 = playersGroup.PlayerByHint(player2Hint);

            // 2) Players must be different.
            if (player1 == player2)
            {
                await context.SendMessage("Diay, jugó solo?");
                return;
            }

            // Everything is correct at this point, go ahead and update the group.
            player1.Match(player1Goals, player2Goals);
            player2.Match(player2Goals, player1Goals);
            context.GroupsRepository.Save(playersGroup);

            // confirmation message
            await context.SendMessage("Anotado!");
        }
    }
}
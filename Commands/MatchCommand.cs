namespace MilkshakeCup.Commands
{
    using System.Linq;
    using System.Threading.Tasks;
    using MilkshakeCup.Models;

    public static class MatchCommand
    {
        public static async Task Execute(CommandContext context)
        {
            var validMatch = context.CreateMatch();

            if (validMatch.HasErrors)
            {
                await context.SendMessage(validMatch.Errors.First());
                return;
            }

            // Everything is correct at this point, go ahead and update the group.
            var match = validMatch.Value;
            var matchGroup = match.Register();
            context.GroupsRepository.Save(matchGroup);

            // confirmation message
            await context.SendMarkdownMessage("Anotado!\n\n" + matchGroup.AsMarkdown());
        }

        public static Valid<Match> CreateMatch(this CommandContext context)
        {
            var validMatch = new Valid<Match>();

            if (context.Parameters.Length != 5)
            {
                validMatch.AddError("Marcador debe llevar: equipo goles equipo goles\nPor ejemplo: Atletico 1 Eibar 0");
            }

            var player1Hint = context.Parameters[1]?.ToLower();
            ushort player1Goals;
            var player2Hint = context.Parameters[3]?.ToLower();
            ushort player2Goals;

            var playersGroup = context.GroupsRepository.Groups().FirstOrDefault(
                group =>
                    group.PlayerByHint(player1Hint) != null &&
                    group.PlayerByHint(player2Hint) != null);

            if (playersGroup == null)
            {
                validMatch.AddError($"no encontré un grupo con: {player1Hint}, {player2Hint}");
            }

            // score validations    
            // 1) goals reported are valid scores        
            if (!ushort.TryParse(context.Parameters[2], out player1Goals))
            {
                validMatch.AddError($"Este marcador esta mamando: {context.Parameters[2]}");
            }

            if (!ushort.TryParse(context.Parameters[4], out player2Goals))
            {
                validMatch.AddError($"Este marcador esta mamando: {context.Parameters[4]}");
            }

            // 2) no one should score more than 10 goals (possible but not probable)
            if (player1Goals > Score.GoalsThreshold || player2Goals > Score.GoalsThreshold)
            {
                validMatch.AddError("Mejor vuelvan a jugarlo, pero esta vez con todos los controles encendidos");
            }

            var player1 = playersGroup.PlayerByHint(player1Hint);
            var player2 = playersGroup.PlayerByHint(player2Hint);

            // 2) Players must be different.
            if (player1 == player2)
            {
                validMatch.AddError("Diay, jugó solo?");
            }

            if (!validMatch.HasErrors)
            {
                validMatch = new Valid<Match>(new Match(new Score(player1, player1Goals), new Score(player2, player2Goals)));
            }

            return validMatch;
        }
    }
}
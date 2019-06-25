namespace MilkshakeCup.Model
{
    using System.Collections.Generic;
    using System.Linq;

    public class MatchCollection
    {
        public string Name { get; }

        private List<Match> matches { get; }

        public MatchCollection(string name)
        {
            Name = name;
            matches = new List<Match>();
        }

        public void Add(Match match)
        {
            matches.Add(match);
        }

        public void Standings()
        {
            // TODO
            // go through Matches and get the standings. This should return an standing object

        }

        public bool HasMatch(Player player, Player otherPlayer)
        {
            return FindMatch(player, otherPlayer) != null || FindMatch(otherPlayer, player) != null;
        }

        public Match Report(Player player1, Player player2, Score score)
        {
            var reportedMatch = FindMatch(player1, player2);
            
            if(reportedMatch == null)
            {
                reportedMatch = FindMatch(player2, player1);
            }

            if(reportedMatch.Score == Score.NotPlayed)
            {
                reportedMatch.Score = score;
            }

            return reportedMatch;
        }

        private Match FindMatch(Player home, Player away)
        {
            return matches.FirstOrDefault(match => match.Home == home && match.Away == away);
        }
    }
}
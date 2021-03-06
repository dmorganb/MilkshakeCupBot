namespace MilkshakeCup.Models
{
    using System.Collections.Generic;
    using System.Linq;

    public class Group
    {
        public string Name { get; }

        // Returns the players ordered by the rules.
        public IOrderedEnumerable<Player> Players =>
            _players
                .OrderByDescending(x => x.Points)
                .ThenByDescending(x => x.GoalDifference)
                .ThenByDescending(x => x.GoalsInFavor);

        private readonly List<Player> _players;

        public Group(string name)
        {
            Name = name;
            _players = new List<Player>();
        }

        public void Add(Player player)
        {
            player.Group = this;
            _players.Add(player);
        }

        /// <summary>
        /// Search for a player in the group by a hint.
        /// hint can be the name of the player or the team.
        /// </summary>
        public Player PlayerByHint(string hint) =>
            _players.FirstOrDefault(x => x.Team.StartsWith(hint) || x.Name.Contains(hint));

        public bool HasPlayer(string hint) => PlayerByHint(hint) != null;
    }
}
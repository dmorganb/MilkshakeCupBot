namespace MilkshakeCup.Model
{
    using System;

    public class Player
    {
        public string Name { get; }

        public string Team { get; }

        public Player(string name, string team)
        {
            Name = name;
            Team = team;
        }

        public bool IsPlayer(string nameOrTeam)
        {
            return AreEqual(Name, nameOrTeam) || AreEqual(Team, nameOrTeam);
        }

        private static bool AreEqual(string source, string value)
        {
            return source.StartsWith(value, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
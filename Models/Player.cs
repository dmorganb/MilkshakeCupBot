namespace MilkshakeCup.Models
{
    public class Player
    {
        public string Name { get; }

        public string Team { get; }

        public ushort Points => (ushort)((Won * 3) + Draw);

        public ushort TotalGames => (ushort)(Won + Lost + Draw);

        public ushort Won { get; private set; }

        public ushort Draw { get; private set; }

        public ushort Lost { get; private set; }

        public ushort GoalsInFavor { get; private set; }

        public ushort GoalsAgainst { get; private set; }

        public ushort GoalDifference => (ushort)(GoalsInFavor - GoalsAgainst);

        public Group Group { get; internal set; }

        public Player(
            string player,
            string team,
            ushort won,
            ushort draw,
            ushort lost,
            ushort goalsInFavor,
            ushort goalsAgainst)
        {
            Name = player;
            Team = team;
            Won = won;
            Draw = draw;
            Lost = lost;
            GoalsInFavor = goalsInFavor;
            GoalsAgainst = goalsAgainst;
        }

        public void Match(ushort goalsInFavor, ushort goalsAgainst)
        {
            if (goalsInFavor > goalsAgainst)
            {
                Won++;
            }
            else if (goalsInFavor < goalsAgainst)
            {
                Lost++;
            }
            else
            {
                Draw++;
            }

            GoalsInFavor += goalsInFavor;
            GoalsAgainst += goalsAgainst;
        }

        public void Revert(ushort goalsInFavor, ushort goalsAgainst)
        {
            if (goalsInFavor > goalsAgainst)
            {
                Won--;
            }
            else if (goalsInFavor < goalsAgainst)
            {
                Lost--;
            }
            else
            {
                Draw--;
            }

            GoalsInFavor -= goalsInFavor;
            GoalsAgainst -= goalsAgainst;
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                var row = (Player)obj;

                return (Name == row.Name) && (Team == row.Team);
            }
        }

        // magic! https://stackoverflow.com/a/34913143/791186
        public override int GetHashCode()
        {
            int hash = 19;

            unchecked // allow "wrap around" in the int
            {
                hash = hash * 31 + this.Name.GetHashCode();
                hash = hash * 31 + this.Team.GetHashCode();
            }

            return hash;
        }
    }
}
namespace MilkshakeCup.Models
{
    public class Player
    {
        public string Name { get; }

        public string Team { get; }

        public int Points => (Won * 3) + Draw;

        public int TotalGames => Won + Lost + Draw;

        public int Won { get; private set; }

        public int Draw { get; private set; }

        public int Lost { get; private set; }

        public int GoalsInFavor { get; private set; }

        public int GoalsAgainst { get; private set; }

        public int GoalDifference => GoalsInFavor - GoalsAgainst;

        public Group Group { get; internal set; }

        public Player(
            string player,
            string team,
            int won,
            int draw,
            int lost,
            int goalsInFavor,
            int goalsAgainst)
        {
            Name = player;
            Team = team;
            Won = won;
            Draw = draw;
            Lost = lost;
            GoalsInFavor = goalsInFavor;
            GoalsAgainst = goalsAgainst;
        }

        public void Match(int goalsInFavor, int goalsAgainst)
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

        public void Erase(int goalsInFavor, int goalsAgainst)
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
namespace MilkshakeCup
{
    public class Row
    {
        public string Player { get; set; }
        
        public string Team { get; set; }
        
        public int Points => (Won * 3) + Draw;

        public int TotalGames => Won + Lost + Draw;

        public int Won { get; set; }

        public int Draw { get; set; }

        public int Lost { get; set; }            
        
        public int GoalsInFavor { get; set; }

        public int GoalsAgainst { get; set; }

        public int GoalDifference => GoalsInFavor - GoalsAgainst;

        public Row(
            string player, 
            string team, 
            int won, 
            int draw, 
            int lost, 
            int goalsInFavor, 
            int goalsAgainst)
        {
            Player = player;
            Team = team;
            Won = won;
            Draw = draw;
            Lost = lost;
            GoalsInFavor = goalsInFavor;
            GoalsAgainst = goalsAgainst;
        }
    }
}
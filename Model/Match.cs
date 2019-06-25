namespace MilkshakeCup.Model
{
    public class Match
    {
        public Player Home { get; }

        public Player Away { get; }

        public Score Score { get; set; }

        public Match(Player home, Player away)
            : this(home, away, Score.NotPlayed)
        {
            
        }

        public Match(Player home, Player away, Score score)
        {
            Home = home;
            Away = away;
            Score = score;
        }
    }
}
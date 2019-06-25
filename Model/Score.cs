namespace MilkshakeCup.Model
{
    public class Score
    {
        public int Home { get; }

        public int Away { get; }

        public static Score NotPlayed = new Score();
    } 
}
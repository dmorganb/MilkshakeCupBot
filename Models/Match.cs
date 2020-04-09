namespace MilkshakeCup.Models
{
    using System;

    public class Match
    {
        public Score Home { get; private set; }

        public Score Away { get; private set; }

        public Group Group => Home.Player.Group;

        public Match(Score home, Score away)
        {
            if (home.Player.Group != away.Player.Group)
            {
                throw new ArgumentException("", nameof(away));
            }

            Home = home;
            Away = away;
        }

        public Group Register()
        {
            Home.Player.Match(Home.Goals, Away.Goals);
            Away.Player.Match(Away.Goals, Home.Goals);
            
            return Group;
        }

        public Group Unregister()
        {
            Home.Player.Revert(Home.Goals, Away.Goals);
            Away.Player.Revert(Away.Goals, Home.Goals);
            
            return Group;
        }
    }
}
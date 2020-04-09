namespace MilkshakeCup.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Score
    {
        // if someone scored more than 10 in the same game, that's suspicious.
        public const int GoalsThreshold = 10;

        public Player Player { get; private set; }

        public ushort Goals { get; private set; }

        public Score(Player player, ushort goals)
        {            
            if (player == null)
            {
                throw new ArgumentNullException(nameof(Player));
            }

            if (goals > GoalsThreshold)
            {
                throw new ArgumentException(nameof(goals));
            }

            Player = player;
            Goals = goals;
        }
    }
}
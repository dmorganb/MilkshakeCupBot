using System.Collections.Generic;
using System.Linq;

namespace MilkshakeCup.Model
{
    public class MatchCollectionBuilder
    {
        private string _name;

        private List<Player> _players;

        private bool _isHomeAndAway;

        public MatchCollectionBuilder()
        {
            _players = new List<Player>();
        }

        public MatchCollectionBuilder AddPlayer(Player player)
        {
            _players.Add(player);
            return this;
        }

        public MatchCollectionBuilder HomeAndAway()
        {
            _isHomeAndAway = true;
            return this;
        }

        public MatchCollectionBuilder Name(string name)
        {
            _name = name;
            return this;
        }

        public MatchCollection Build()
        {
            var matchCollection = new MatchCollection(_name);

            foreach (var player in _players)
            {
                foreach (var otherPlayer in _players.Where(p => p != player))
                {
                    matchCollection.Add(new Match(player, otherPlayer, Score.NotPlayed));

                    if(_isHomeAndAway)
                    {
                        matchCollection.Add(new Match(otherPlayer, player, Score.NotPlayed));
                    }
                }
                
            }

            return matchCollection;
        }
    }
}
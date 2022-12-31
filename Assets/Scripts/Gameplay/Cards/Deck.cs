using System.Collections.Generic;
using System.Linq;
using Random = System.Random;

namespace Strid.Gameplay.Cards {
    public class Deck {
        private static readonly Random Rng = new Random();
        private readonly List<Card> _cards;

        private Deck() { _cards = new List<Card>(); }
        public Deck(IEnumerable<Card> cards) : this() { Populate(cards); }

        public IEnumerable<Card> Reverse() { return Enumerable.Reverse(_cards); }

        /// <summary>
        /// Shuffle based on the Fisher-Yates shuffle<br />
        /// Inspired by a response from grenade and Uwe Keim on StackOverFlow
        /// SOURCE : https://stackoverflow.com/a/1262619
        /// LAST ACCESSED : 01/10/2022
        /// </summary>
        public void Shuffle() {
            var n = _cards.Count;

            while (n > 1) {
                n--;
                var k = Rng.Next(n + 1);
                (_cards[k], _cards[n]) = (_cards[n], _cards[k]);
            }
        }

        public List<Card> Draw(int amount) {
            var drawn = new List<Card>(_cards.GetRange(0, amount));
            _cards.RemoveRange(0, amount);
            return drawn;
        }

        private void Populate(IEnumerable<Card> cards) { _cards.AddRange(cards); }
    }
}
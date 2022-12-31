using System.Collections.Generic;

namespace Strid.Gameplay.Cards {
    public class Graveyard {
        private readonly List<Card> _cards;
        
        public Graveyard() { _cards = new List<Card>(); }

        public void DestroyCard(Card card) { _cards.Add(card); }

        // TODO: Implement resurrection (Special card)
        public Card PlayCard(int id) {
            var card = _cards[id];
            _cards.RemoveAt(id);
            return card;
        }
    }
}
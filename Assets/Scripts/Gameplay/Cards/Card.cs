using System;
using UnityEngine;
using UnityEngine.Events;

namespace Strid.Gameplay.Cards {
    using Utility;
    using Utility.Storage;

    public enum CardType { Unit, Spy, UnitBuff, LineBuff, Weather, Decoy, Special, }
    public enum CardLine { Melee, Range, Siege, Any, }

    [Serializable]
    public class Card : Loggable, IStorable {
        public int cardId = -1;
        public string title = "";
        public Sprite artwork;
        
        // Stats
        public CardType type = CardType.Unit;
        public CardLine line = CardLine.Any;
        public int combatPower = -1;

        public UnityEvent onPlayCard;

        private void OnValidate() {
            combatPower = Mathf.Max(2, Mathf.Min(combatPower, 10));
            if (type == CardType.Spy) combatPower = 10;
            if (type == CardType.UnitBuff) line = CardLine.Any;
        }

        #region Overrides

            #region Loggable

            protected override string ToLog() { return $"CardPrefab {cardId}: [{combatPower}] - {title} ({type}, {line})"; }

            #endregion

            #region IStorable

            public void SetId(int id) { cardId = id; }

            public int GetId() { return cardId; }

            #endregion

            public override string ToString() { return $"[{combatPower}] - {title} ({type}, {line})"; }

        #endregion
    }
}
using System;
using UnityEngine;

namespace Strid.Gameplay.Cards {
    using Utility;
    using Utility.Storage;

    [Serializable] public enum CardType { Unit, Spy, UnitBuff, LineBuff, Weather, Decoy, Special, }
    
    [Serializable] public enum CardLine { Melee, Range, Siege, Any, }

    [Serializable]
    public class Card : Loggable, IStorable {
        public int cardId = -1;
        public string title = "";
        public Texture2D artwork;
        
        // Stats
        public CardType type = CardType.Unit;
        public CardLine line = CardLine.Any;
        public int combatPower = -1;

        public MaterialPropertyBlock GetArtwork() {
            var block = new MaterialPropertyBlock();
            block.SetTexture(Shader.PropertyToID("_MainTex"), artwork);
            return block;
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
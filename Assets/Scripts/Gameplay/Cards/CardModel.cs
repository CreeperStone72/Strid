using System;
using TMPro;
using UnityEngine;

namespace Strid.Gameplay.Cards {
    public class CardModel : MonoBehaviour {
        public TMP_Text combatPower;
        public SpriteRenderer portrait;
        public TMP_Text title;
        public SpriteRenderer type, line;

        [Header("Type icons")]
        public Sprite unit;
        public Sprite spy;
        public Sprite unitBuff;
        public Sprite lineBuff;
        public Sprite weather;
        public Sprite decoy;
        public Sprite special;

        [Header("Line icons")]
        public Sprite melee;
        public Sprite range;
        public Sprite siege;
        public Sprite any;

        public void Initialize(Card reference) {
            combatPower.text = reference.combatPower.ToString();
            portrait.sprite = reference.artwork;
            title.text = reference.title;
            
            // TODO: Implement type icons
            type.sprite = reference.type switch {
                CardType.Unit => unit,
                CardType.Spy => spy,
                CardType.UnitBuff => unitBuff,
                CardType.LineBuff => lineBuff,
                CardType.Weather => weather,
                CardType.Decoy => decoy,
                CardType.Special => special,
                _ => throw new ArgumentOutOfRangeException()
            };

            // TODO: Implement line icons
            line.sprite = reference.line switch {
                CardLine.Melee => melee,
                CardLine.Range => range,
                CardLine.Siege => siege,
                CardLine.Any => any,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
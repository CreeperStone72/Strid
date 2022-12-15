using System;
using System.Collections.Generic;
using System.Linq;
using Strid.Gameplay.Cards;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Strid {
    public class PlayerLinesPrefab : MonoBehaviour, IPointerClickHandler {
        public PlayerHandPrefab Hand => hand;
        public bool humanPlayer = true;
        
        [SerializeField] private PlayerHandPrefab hand;
        
        [Header("Lines")]
        [SerializeField] private LinePrefab melee;
        [SerializeField] private LinePrefab range;
        [SerializeField] private LinePrefab siege;
        
        [Header("Click areas")]
        [SerializeField] private GameObject meleeArea;
        [SerializeField] private GameObject rangeArea;
        [SerializeField] private GameObject siegeArea;

        private LinePrefab _clicked;
        
        private void Start() {
            if (humanPlayer) {
                meleeArea.SetActive(false);
                rangeArea.SetActive(false);
                siegeArea.SetActive(false);
            }
            _clicked = null;
        }

        public void Play(UICardPrefab card) {
            var model = card.model;

            var pickLine = humanPlayer ? (Func<LinePrefab>) PickLine : PickLineRandom;

            switch (model.type) {
                case CardType.Unit:     GetLine(model.line, pickLine).AddUnit(card);                       break;
                // TODO: Implement proper spies
                case CardType.Spy:      GetLine(model.line, pickLine).AddUnit(card);                       break;
                case CardType.UnitBuff: GetLine(model.line, pickLine).BuffUnit(card);                      break;
                case CardType.LineBuff: GetLine(model.line, pickLine).BuffLine(card);                      break;
                case CardType.Weather:  GetLine(model.line).Weather(card);                                 break;
                case CardType.Decoy:    hand.AddCardToHand(GetLine(model.line).AddDecoy(card).gameObject); break;
                // TODO: Implement proper special cards
                case CardType.Special:  GetLine(model.line).AddUnit(card);                                 break;
                default:                throw new ArgumentOutOfRangeException();
            }
        }
        
        public void OnPointerClick(PointerEventData eventData) {
            if (!humanPlayer) return;
            var target = eventData.pointerCurrentRaycast.gameObject.name;

            if (melee.name == target) _clicked = melee;
            else if (range.name == target) _clicked = range;
            else if (siege.name == target) _clicked = siege;
            else _clicked = null;
        }

        public List<GameObject> ClearLines() {
            var cards = new List<UICardPrefab>();
            cards.AddRange(melee.Clear());
            cards.AddRange(range.Clear());
            cards.AddRange(siege.Clear());
            return cards.Select(card => card.gameObject).ToList();
        }

        public int[] GetCombatPowers() {
            var powers = new int[3];
            powers[0] = melee.GetCombatPower();
            powers[1] = range.GetCombatPower();
            powers[2] = siege.GetCombatPower();
            return powers;
        }
        
        private LinePrefab PickLine() {
            meleeArea.SetActive(true);
            rangeArea.SetActive(true);
            siegeArea.SetActive(true);

            while (_clicked == null) { }

            meleeArea.SetActive(false);
            rangeArea.SetActive(false);
            siegeArea.SetActive(false);

            return _clicked;
        }

        private LinePrefab PickLineRandom() {
            var prng = new System.Random().Next(3);

            return prng switch {
                0 => melee,
                1 => range,
                2 => siege,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private LinePrefab GetLine(CardLine line, Func<LinePrefab> onLinePick = null) {
            return line switch {
                CardLine.Melee => melee,
                CardLine.Range => range,
                CardLine.Siege => siege,
                CardLine.Any when onLinePick == null => throw new ArgumentException(),
                CardLine.Any => onLinePick(),
                _ => throw new ArgumentOutOfRangeException(nameof(line), line, null)
            };
        }
    }
}
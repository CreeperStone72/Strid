using System;
using System.Collections.Generic;
using System.Linq;
using Strid.Gameplay.Cards;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Strid {
    [RequireComponent(typeof(ScrollRect))]
    public class LinePrefab : MonoBehaviour, IPointerClickHandler {
        [SerializeField] private TMP_Text lineStrength;
        [SerializeField] private Transform lineBuff;
        [SerializeField] private Transform weather;

        private List<UICardPrefab> cards;
        private UICardPrefab _clicked;

        private void Awake() {
            cards = new List<UICardPrefab>();
            _clicked = null;
        }
        
        private void AddCard(UICardPrefab card) {
            cards.Add(card);
            card.transform.parent = transform;
        }

        public int GetCombatPower() {
            var factor = 1;
            var sum = 0;

            foreach (var card in cards.Select(card => card.model)) {
                switch (card.type) {
                    case CardType.Unit: case CardType.Spy: sum += card.combatPower;           break;
                    case CardType.UnitBuff:                                                   break;
                    case CardType.LineBuff:                factor = factor != 0 ? 2 : factor; break;
                    case CardType.Weather:                 factor = 0;                        break;
                    case CardType.Decoy:                                                      break;
                    case CardType.Special: default:        throw new ArgumentOutOfRangeException();
                }
            }

            return sum * factor;
        }

        public void AddUnit(UICardPrefab card) {
            AddCard(card);
            OnPlayedCard();
        }

        public void BuffUnit(UICardPrefab card) {
            AddCard(card);
            while (_clicked == null) { }
            card.transform.parent = _clicked.transform;
            _clicked.model.combatPower += card.model.combatPower;
            card.model.combatPower = 0;
            OnPlayedCard();
        }

        public void BuffLine(UICardPrefab card) {
            AddCard(card);
            card.transform.parent = lineBuff;
            OnPlayedCard();
        }

        public void Weather(UICardPrefab card) {
            AddCard(card);
            card.transform.parent = weather;
            OnPlayedCard();
        }

        public UICardPrefab AddDecoy(UICardPrefab card) {
            AddCard(card);
            while (_clicked == null) { }
            (card.transform.parent, _clicked.transform.parent) = (_clicked.transform.parent, card.transform.parent);
            cards.Remove(_clicked);
            OnPlayedCard();
            return _clicked;
        }

        public List<UICardPrefab> Clear() {
            var cleared = cards;
            cards.Clear();
            return cleared;
        }
        
        public void OnPointerClick(PointerEventData eventData) {
            var target = eventData.pointerCurrentRaycast.gameObject.name;
            string N(Component card) => card.gameObject.name;
            foreach (var card in from card in cards where card.IsVisible && card.IsInFocus && N(card) == target select card) _clicked = card;
        }

        private void OnPlayedCard() {
            lineStrength.text = GetCombatPower() + "";
        }
    }
}
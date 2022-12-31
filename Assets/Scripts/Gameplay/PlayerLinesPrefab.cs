using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Strid.Gameplay {
    using Cards;
    using Utility;
    
    public class PlayerLinesPrefab : MonoBehaviour, IPointerClickHandler {
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

        public async Task Play(UICardPrefab card) {
            if (humanPlayer) await PickLine(card).ContinueWith(_ => PlayCard(card, GetLine(card.model.line)));
            else await PlayCard(card, PickLineRandom());
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

        private async Task PlayCard(UICardPrefab card, LinePrefab line) {
            var model = card.model;

            switch (model.type) {
                case CardType.Unit:     line.AddUnit(card);                                 break;
                // TODO: Implement proper spies
                case CardType.Spy:      line.AddUnit(card);                                 break;
                case CardType.UnitBuff: await line.BuffUnit(card);                          break;
                case CardType.LineBuff: line.BuffLine(card);                                break;
                case CardType.Weather:  line.Weather(card);                                 break;
                case CardType.Decoy:    hand.AddCardToHand(line.AddDecoy(card).gameObject); break;
                // TODO: Implement proper special cards
                case CardType.Special:  line.AddUnit(card);                                 break;
                default:                throw new ArgumentOutOfRangeException();
            }
        }
        
        private async Task PickLine(UICardPrefab card) {
            if (card.model.line != CardLine.Any) return;
            
            meleeArea.SetActive(true);
            rangeArea.SetActive(true);
            siegeArea.SetActive(true);

            await TaskUtils.WaitUntil(() => _clicked != null);

            meleeArea.SetActive(false);
            rangeArea.SetActive(false);
            siegeArea.SetActive(false);
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

        private LinePrefab GetLine(CardLine line) {
            return line switch {
                CardLine.Melee => melee,
                CardLine.Range => range,
                CardLine.Siege => siege,
                CardLine.Any => _clicked,
                _ => throw new ArgumentOutOfRangeException(nameof(line), line, null)
            };
        }
    }
}
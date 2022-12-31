using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Strid.Gameplay {
    using Cards;
    using Utility;
    
    [RequireComponent(typeof(ScrollRect))]
    public class LinePrefab : MonoBehaviour, IPointerClickHandler {
        [SerializeField] private TMP_Text lineStrength;
        [SerializeField] private Transform lineBuff;
        [SerializeField] private Transform weather;
        [SerializeField] private Transform cards;

        private List<UICardPrefab> _cards;
        private UICardPrefab _clicked;

        private void Awake() {
            _cards = new List<UICardPrefab>();
            _clicked = null;
            lineBuff.gameObject.SetActive(false);
            weather.gameObject.SetActive(false);
        }
        
        private void AddCard(UICardPrefab card) {
            _cards.Add(card);
            Debug.Log($"Added Card {card.model}");
            // BUG: SetParent is not working
            card.transform.SetParent(cards);
            Debug.Log($"Attached ${card.model} to the line !");
            StartCoroutine(MoveCard(card.transform));
        }

        public int GetCombatPower() {
            var factor = 1;
            var sum = 0;

            foreach (var card in _cards.Select(card => card.model)) {
                switch (card.type) {
                    case CardType.Unit: case CardType.Spy: sum += card.combatPower;           break;
                    case CardType.UnitBuff:                                                   break;
                    case CardType.LineBuff:                factor = factor != 0 ? 2 : factor; break;
                    case CardType.Weather:                 factor = 0;                        break;
                    case CardType.Decoy:                                                      break;
                    case CardType.Special: default:        sum += card.combatPower;           break;
                }
            }

            return sum * factor;
        }

        public void AddUnit(UICardPrefab card) {
            AddCard(card);
            OnPlayedCard();
        }

        public async Task BuffUnit(UICardPrefab card) {
            AddCard(card);
            
            await TaskUtils.WaitUntil(() => _clicked != null);
            
            card.transform.parent = _clicked.transform;
            _clicked.model.combatPower += card.model.combatPower;
            card.model.combatPower = 0;
            OnPlayedCard();
        }

        public void BuffLine(UICardPrefab card) {
            AddCard(card);
            lineBuff.gameObject.SetActive(true);
            OnPlayedCard();
        }

        public void Weather(UICardPrefab card) {
            AddCard(card);
            weather.gameObject.SetActive(card.model.line != CardLine.Any);
            OnPlayedCard();
        }

        public UICardPrefab AddDecoy(UICardPrefab card) {
            AddCard(card);
            while (_clicked == null) { }

            var (t1, t2) = (_clicked.transform, card.transform);
            (t1.parent, t2.parent) = (t2.parent, t1.parent);
            
            _cards.Remove(_clicked);
            OnPlayedCard();
            return _clicked;
        }

        public IEnumerable<UICardPrefab> Clear() {
            var cleared = _cards;
            _cards.Clear();
            return cleared;
        }
        
        public void OnPointerClick(PointerEventData eventData) {
            var target = eventData.pointerCurrentRaycast.gameObject.name;
            foreach (var card in from card in _cards where card.IsVisible && card.IsInFocus && card.gameObject.name == target select card) _clicked = card;
        }

        private void OnPlayedCard() {
            lineStrength.text = GetCombatPower() + "";
        }

        private IEnumerator MoveCard(Transform card) {
            Debug.Log("MoveCard START");
            var start = card.position;
            var end = cards.position;

            var elapsedTime = 0f;

            while (elapsedTime < 0.2f) {
                card.position = Vector3.Lerp(start, end, elapsedTime / 0.2f);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            card.position = end;
            
            Debug.Log("MoveCard END");
        }
    }
}
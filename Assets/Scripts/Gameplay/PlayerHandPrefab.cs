using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Strid.Gameplay {
    using Cards;
    
    [RequireComponent(typeof(ScrollRect))]
    public class PlayerHandPrefab : MonoBehaviour, IPointerClickHandler {
        public bool isTurn;
        public bool TurnEnded { get; private set; }
        
        public bool humanPlayer = true;

        public int CardCount => _cards.Count;

        public OnPlay onPlay;
        public delegate Task OnPlay();
        
        [SerializeField] private PlayerLinesPrefab playerLines;
        [SerializeField] private GameObject hand;
        [SerializeField] private TMP_Text description;
        [SerializeField] private ScrollRect scroll;

        private List<UICardPrefab> _cards;

        private void OnEnable() {
            _cards = new List<UICardPrefab>();
            if (humanPlayer) scroll.onValueChanged.AddListener(UpdateCardFocus);
        }

        private void OnDisable() { if (humanPlayer) scroll.onValueChanged.RemoveListener(UpdateCardFocus); }
        
        public void AddCardToHand(GameObject card) {
            _cards.Add(card.GetComponent<UICardPrefab>());
            card.transform.SetParent(hand.transform);
        }

        public void StartRound(GraveyardPrefab graveyard) {
            graveyard.DestroyCards(playerLines.ClearLines());
            TurnEnded = false;
        }

        public void EndTurn() {
            isTurn = false;
            TurnEnded = true;
        }

        private void UpdateCardFocus(Vector2 position) {
            foreach (var card in _cards) {
                if (card.IsCentered) card.OnFocusEnter(ref description);
                else if (card.IsInFocus) card.OnFocusExit();
            }
        }

        public async Task PlayCard(int id) {
            if (humanPlayer) return;
            if (0 > id || id >= _cards.Count) return;
            Debug.Log($"Playing {_cards[id]}");
            await playerLines.Play(_cards[id]);
            isTurn = false;
            await onPlay();
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (!isTurn || !humanPlayer) return;

            UICardPrefab played = null;
            
            var target = eventData.pointerCurrentRaycast.gameObject.name;
            foreach (var card in from card in _cards where card.IsVisible && card.gameObject.name == target select card) played = card;

            if (played == null) return;
            
            Debug.Log($"Playing {played}");
            
            playerLines.Play(played).ContinueWith(_ => {
                isTurn = false;
                onPlay();
            });
        }
    }
}
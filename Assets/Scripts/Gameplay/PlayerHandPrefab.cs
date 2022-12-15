using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Strid {
    [RequireComponent(typeof(ScrollRect))]
    public class PlayerHandPrefab : MonoBehaviour, IPointerClickHandler {
        public bool isTurn;
        public bool TurnEnded { get; private set; }
        
        public bool humanPlayer = true;

        public int CardCount => cards.Count;

        public OnPlay onPlay;
        public delegate void OnPlay();
        
        [SerializeField] private PlayerLinesPrefab playerLines;
        [SerializeField] private GameObject hand;
        [SerializeField] private TMP_Text description;

        private List<UICardPrefab> cards;
        private ScrollRect scroll;

        private void Start() {
            cards = new List<UICardPrefab>();
            scroll = FindObjectOfType<ScrollRect>();

            if (humanPlayer) scroll.onValueChanged.AddListener(UpdateCardFocus);
        }

        private void OnDestroy() { if (humanPlayer) scroll.onValueChanged.RemoveListener(UpdateCardFocus); }
        
        public void AddCardToHand(GameObject card) {
            cards.Add(card.GetComponent<UICardPrefab>());
            card.transform.parent = hand.transform;
        }

        public void RemoveCardFromHand(GraveyardPrefab graveyard, UICardPrefab card) {
            RemoveCardsFromHand(graveyard, new List<UICardPrefab> { card });
        }

        public void RemoveCardsFromHand(GraveyardPrefab graveyard, List<UICardPrefab> destroyedCards) {
            graveyard.DestroyCards(destroyedCards.Select(card => card.gameObject).ToList());
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
            foreach (var card in cards) {
                if (card.IsCentered) card.OnFocusEnter(ref description);
                else if (card.IsInFocus) card.OnFocusExit();
            }
        }

        public void PlayCard(int id) {
            if (humanPlayer) return;
            if (0 > id || id >= cards.Count) return;
            playerLines.Play(cards[id]);
            isTurn = false;
            onPlay();
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (!isTurn || !humanPlayer) return;
            var target = eventData.pointerCurrentRaycast.gameObject.name;
            string N(Component card) => card.gameObject.name;
            foreach (var card in from card in cards where card.IsVisible && card.IsInFocus && N(card) == target select card) playerLines.Play(card);
            isTurn = false;
            onPlay();
        }
    }
}
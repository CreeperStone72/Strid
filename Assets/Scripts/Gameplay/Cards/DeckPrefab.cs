using System.Collections;
using System.Linq;
using UnityEngine;

namespace Strid {
    using Gameplay.Cards;
    
    public class DeckPrefab : MonoBehaviour {
        public bool setupDone = false;
        private Deck _deck;

        [SerializeField] private float cardOffset = 0.01f;
        [SerializeField] private float spaceBetweenCards = 0.1f;
        [SerializeField] private float timeToSlide = 0.2f;

        [SerializeField] private PlayerHandPrefab playerHand;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject uiCard;
        [SerializeField] private GameObject cardPrefab;
        
        public void Setup(Deck deck) {
            _deck = deck;
            var position = transform.position;
            
            foreach (var card in _deck.Reverse()) {
                var prefab = Instantiate(cardPrefab, transform);
                prefab.transform.position = position;
                prefab.GetComponent<CardPrefab>().model = card;
                prefab.GetComponent<CardPrefab>().faceUp = false;

                position += new Vector3(0, -cardOffset, spaceBetweenCards);
            }

            setupDone = true;
        }

        public void SetPlayer(PlayerHandPrefab player) { playerHand = player; }

        public void Draw(int amount = 1, bool flip = false) {
            var drawn = _deck.Draw(amount);

            foreach (var c in from card in drawn from cardPrefab in GetCards() where cardPrefab.model == card select cardPrefab) {
                StartCoroutine(TransferCard(c, flip));
            }
        }

        private IEnumerator TransferCard(CardPrefab drawn, bool flip) {
            if (flip) drawn.Flip();
            yield return new WaitForSeconds(drawn.FlipTime);

            var start = drawn.transform.position;
            var end = playerHand.transform.position;
            var elapsedTime = 0f;

            while (elapsedTime < timeToSlide) {
                drawn.transform.position = Vector3.Lerp(start, end, elapsedTime / timeToSlide);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            drawn.transform.position = end;
            var card = Instantiate(uiCard, end, Quaternion.identity, playerHand.transform);
            card.GetComponent<UICardPrefab>().SetModel(drawn.model);
            Destroy(drawn);
            
            playerHand.AddCardToHand(card);
        }

        private CardPrefab[] GetCards() { return transform.GetComponentsInChildren<CardPrefab>(); }
    }
}
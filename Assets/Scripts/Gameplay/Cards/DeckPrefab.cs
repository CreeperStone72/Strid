using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Strid.Gameplay.Cards {
    public class DeckPrefab : MonoBehaviour {
        private Deck _deck;

        [SerializeField] private float cardOffset = 0.01f;
        [SerializeField] private float spaceBetweenCards = 0.1f;
        [SerializeField] private float timeToSlide = 0.2f;

        [Header("Prefabs")]
        [SerializeField] private GameObject uiCard;
        [SerializeField] private GameObject cardPrefab;
        
        public void Setup(Deck deck) {
            _deck = deck;
            var position = transform.position;
            
            foreach (var card in _deck.Reverse()) {
                var prefab = Instantiate(cardPrefab, transform);
                prefab.transform.position = position;
                prefab.GetComponent<CardPrefab>().OnStart(card, false);

                position += new Vector3(0, -cardOffset, spaceBetweenCards);
            }
        }

        public void Draw(PlayerHandPrefab player, int amount = 1) {
            var drawn = _deck.Draw(amount);
            foreach (var c in from card in drawn from cardPrefab in GetCards() where cardPrefab.model == card select cardPrefab) StartCoroutine(TransferCard(player, c));
        }

        private IEnumerator TransferCard(PlayerHandPrefab player, CardPrefab drawn) {
            #region Reveal the card (if drawn for the human player)

            if (player.humanPlayer) drawn.Flip();
            yield return new WaitForSeconds(drawn.FlipTime);

            #endregion

            #region Move the CardPrefab

            var start = drawn.transform.position;
            var end = player.transform.position;
            var elapsedTime = 0f;

            while (elapsedTime < timeToSlide) {
                drawn.transform.position = Vector3.Lerp(start, end, elapsedTime / timeToSlide);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            drawn.transform.position = end;

            #endregion

            #region Create the UICardPrefab (for the player hand)

            var card = Instantiate(uiCard, end, Quaternion.identity, player.transform);
            card.GetComponent<UICardPrefab>().SetModel(drawn.model);
            Destroy(drawn);

            #endregion

            player.AddCardToHand(card);
        }

        private IEnumerable<CardPrefab> GetCards() { return transform.GetComponentsInChildren<CardPrefab>(); }
    }
}
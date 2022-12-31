using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Strid.Gameplay.Cards {
    public class GraveyardPrefab : MonoBehaviour {
        private Graveyard _graveyard;

        [SerializeField] private float cardOffset = 0.01f;
        [SerializeField] private float spaceBetweenCards = 0.1f;
        [SerializeField] private float timeToSlide = 0.2f;

        [Header("Prefabs")]
        [SerializeField] private GameObject cardPrefab;
        
        private Vector3 _cardPosition;
        
        public void Setup(Graveyard graveyard) {
            _graveyard = graveyard;
            _cardPosition = transform.position;
        }

        public void DestroyCards(List<GameObject> cards) { foreach (var card in cards) StartCoroutine(TransferCard(card)); }

        private IEnumerator TransferCard(GameObject card) {
            _graveyard.DestroyCard(card.GetComponent<UICardPrefab>().model);

            #region Create 3D card

            var drawn = Instantiate(cardPrefab, card.transform.position, Quaternion.identity, transform);
            drawn.GetComponent<CardPrefab>().model.artwork = card.GetComponent<UICardPrefab>().model.artwork;
            drawn.GetComponent<CardPrefab>().Flip();
            yield return new WaitForSeconds(drawn.GetComponent<CardPrefab>().FlipTime);

            #endregion
            
            Destroy(card);

            #region Move 3D card to graveyard

            var start = drawn.transform.position;
            var end = _cardPosition;
            var elapsedTime = 0f;

            while (elapsedTime < timeToSlide) {
                drawn.transform.position = Vector3.Lerp(start, end, elapsedTime / timeToSlide);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            drawn.transform.position = end;
            _cardPosition += new Vector3(0, -cardOffset, spaceBetweenCards);

            #endregion
        }
    }
}
using System.Collections;
using UnityEngine;

namespace Strid.Gameplay.Cards {
    /// <summary>Model used for pseudo-3D effects with the deck and the graveyard</summary>
    public class CardPrefab : MonoBehaviour {
        public float FlipTime => timeToFlip;
        private bool IsFlippedDown => transform.localRotation.eulerAngles.y == 0f;
        private float Rotation => IsFlippedDown ? 180f : 0f;
        
        [SerializeField] private Sprite backTexture;
        [SerializeField] private SpriteRenderer front, back;
        [SerializeField] private float timeToFlip = 1.5f;

        public bool faceUp;
        public Card model;

        public void OnStart(Card m, bool f) {
            model = m;
            faceUp = f;
            Debug.Log(m);
            front.SetPropertyBlock(m.GetArtwork());
            back.sprite = backTexture;
        }

        private void OnValidate() { transform.localRotation = Quaternion.Euler(0, faceUp ? -180f : 0f, 0); }

        public void Flip() { StartCoroutine(RotateCard()); }

        private IEnumerator RotateCard() {
            var start = transform.localRotation.eulerAngles.y;
            var end = Rotation;
            var elapsedTime = 0f;

            while (elapsedTime < timeToFlip) {
                var euler = Vector3.up * Mathf.Lerp(start, end, elapsedTime / timeToFlip);
                transform.localRotation = Quaternion.Euler(euler);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            transform.localRotation = Quaternion.Euler(Vector3.up * end);
        }
    }
}
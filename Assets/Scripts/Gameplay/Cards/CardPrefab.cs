using System.Collections;
using UnityEngine;

namespace Strid {
    using Gameplay.Cards;
    
    public class CardPrefab : MonoBehaviour {
        public Bounds Bounds => front.sprite.bounds;

        public float FlipTime => timeToFlip;
        
        private bool IsFlippedDown => transform.localRotation.eulerAngles.y == 0f;

        private float Rotation => IsFlippedDown ? 180f : 0f;
        
        public bool faceUp;

        public Card model;

        [SerializeField] private Sprite backTexture;

        [SerializeField] private SpriteRenderer front, back;

        [SerializeField] private float timeToFlip = 1.5f;

        // private Coroutine _test;

        private void Start() {
            front.sprite = model.artwork;
            back.sprite = backTexture;
            // _test = null;
        }

        // private void Update() { test ??= StartCoroutine(Test()); }

        private void OnValidate() { transform.localRotation = Quaternion.Euler(0, faceUp ? -180f : 0f, 0); }

        /*
        private IEnumerator Test() {
            Flip();
            yield return new WaitForSeconds(timeToFlip * 2);
            _test = null;
        }
        */
        
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

        public void SetParent(Transform newParent) { transform.parent = newParent; }
    }
}
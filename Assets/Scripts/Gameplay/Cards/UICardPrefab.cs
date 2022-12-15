using System.Collections;
using Strid.Gameplay.Cards;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Strid {
    [RequireComponent(typeof(Image))]
    public class UICardPrefab : MonoBehaviour {
        public bool IsInFocus { get; private set; }
        public bool IsVisible => 0 <= ScreenPosition && ScreenPosition <= 1;
        public bool IsCentered => .5f - margin <= ScreenPosition && ScreenPosition <= .5f + margin;
        private float ScreenPosition => Camera.main!.WorldToViewportPoint(transform.position).x;
        
        public Card model;

        [SerializeField] private float margin;
        [SerializeField] private float offset;
        
        private Image _front;
        
        private void Start() {
            _front = FindObjectOfType<Image>();
            IsInFocus = false;
        }

        public void SetModel(Card c) {
            model = c;
            _front.sprite = model.artwork;
        }

        public void OnFocusEnter(ref TMP_Text description) {
            IsInFocus = true;
            description.text = model.ToString();
            StartCoroutine(Slide(true));
        }

        public void OnFocusExit() {
            IsInFocus = false;
            StartCoroutine(Slide(false));
        }

        private IEnumerator Slide(bool slideUp) {
            var start = slideUp ? transform.position : transform.position + Vector3.up * offset;
            var end = slideUp ? start + Vector3.up * offset : transform.position;
            var elapsedTime = 0f;

            while (elapsedTime < 0.5f) {
                transform.position = Vector3.Lerp(start, end, elapsedTime / 0.5f);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = end;
        }
    }
}
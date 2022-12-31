using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Strid.Gameplay.Cards {
    /// <summary>Model used for Canvas-based display (player hand and lines)</summary>
    [RequireComponent(typeof(Image))]
    public class UICardPrefab : MonoBehaviour {
        public bool IsInFocus { get; private set; }
        // For some reason, [0 ; 60] is the on-screen interval, even with the normalized WorldToViewportPoint
        public bool IsVisible => 0 <= ScreenPosition && ScreenPosition <= 60;
        public bool IsCentered => 30f - margin <= ScreenPosition && ScreenPosition <= 30f + margin;
        private float ScreenPosition => Camera.main!.WorldToViewportPoint(transform.position).x;
        
        [SerializeField] private float margin;
        
        public Card model;
        private Image _front;
        
        public void SetModel(Card c) {
            name = c.title;
            _front = GetComponent<Image>();
            IsInFocus = false;
            
            model = c;
            _front.sprite = Sprite.Create(model.artwork, new Rect(0.0f, 0.0f, model.artwork.width, model.artwork.height), new Vector2(0.5f, 0.5f));
        }

        public void OnFocusEnter(ref TMP_Text description) {
            IsInFocus = true;
            description.text = model.ToString();
        }

        public void OnFocusExit() { IsInFocus = false; }
    }
}
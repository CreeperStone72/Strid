using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Strid.Network {
    using Firebase;
    using Gameplay.Cards;
    
    public class FirebaseCardStorage : FirebaseDatabaseStorage<Card> {
        [Serializable]
        private class CardStorage {
            public int cardId;
            public string title;
            public CardType type;
            public CardLine line;
            public int combatPower;

            public CardStorage(Card card) {
                cardId = card.cardId;
                title = card.title;
                type = card.type;
                line = card.line;
                combatPower = card.combatPower;
            }
        }
        
        private const string Table = "cards";

        private readonly FirebaseImageStorage _artworks;

        public FirebaseCardStorage() : base(Table) { _artworks = new FirebaseImageStorage(Table); }
        
        protected override string ToJson(Card obj) {
            _artworks.Upload(obj.title, obj.artwork);
            return JsonUtility.ToJson(new CardStorage(obj));
        }

        protected override async Task<Card> FromJson(string json) {
            var c = JsonUtility.FromJson<CardStorage>(json);
            var task = _artworks.Download(c.title);

            await task;
            
            var bytes = task.Result;
            
            var card = new Card {
                cardId = c.cardId,
                title = c.title,
                artwork = new Texture2D(500, 726, TextureFormat.RGBA32, false),
                type = c.type,
                line = c.line,
                combatPower = c.combatPower
            };
            
            card.artwork.LoadRawTextureData(bytes);
            card.artwork.Apply();

            Debug.Log(card);
            
            return card;
        }
    }
}
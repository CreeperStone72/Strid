using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Strid {
    using Gameplay;
    using Gameplay.Cards;
    
    public class GameStateManager : MonoBehaviour {
        private enum Winner {
            Player1,
            Player2,
            Tie,
            ToBeDetermined
        }

        [Header("Common areas")] [SerializeField]
        private DeckPrefab deck;

        [SerializeField] private GraveyardPrefab graveyard;

        [Header("Player areas")] [SerializeField]
        private PlayerHandPrefab playerHand;

        [SerializeField] private PlayerHandPrefab enemyHand;
        [SerializeField] private PlayerLinesPrefab playerLines;
        [SerializeField] private PlayerLinesPrefab enemyLines;

        [Header("Results")] [SerializeField] private GameObject resultScreen;
        [SerializeField] private TMP_Text resultText;

        private Winner[] _scores;
        private bool _isPlayer1Playing;

        private int _round;

        private void Start() {
            resultScreen.SetActive(false);

            deck.Setup(new Deck(GameSetup.cards));
            graveyard.Setup(new Graveyard());

            enemyHand.humanPlayer = false;
            enemyLines.humanPlayer = false;

            playerHand.isTurn = false;
            enemyHand.isTurn = false;

            playerHand.onPlay = TurnPlayed;
            enemyHand.onPlay = TurnPlayed;

            _scores = new[] {Winner.ToBeDetermined, Winner.ToBeDetermined, Winner.ToBeDetermined};
            _isPlayer1Playing = false;

            _round = 1;

            SetupRound();
        }

        private void SetupRound() {
            var dealt = _round switch {1 => 11, 2 => 2, 3 => 1, _ => 0};

            playerHand.StartRound(graveyard);
            enemyHand.StartRound(graveyard);

            deck.Draw(playerHand, dealt);
            deck.Draw(enemyHand, dealt);

            if (_round == 1) {
                playerHand.isTurn = true;
                enemyHand.isTurn = false;
                _isPlayer1Playing = true;
            }
            else
                switch (_scores[_round - 2]) {
                    case Winner.Player1:
                        playerHand.isTurn = true;
                        enemyHand.isTurn = false;
                        _isPlayer1Playing = true;
                        break;
                    case Winner.Player2:
                        playerHand.isTurn = false;
                        enemyHand.isTurn = true;
                        _isPlayer1Playing = false;
                        break;
                    case Winner.Tie:
                        playerHand.isTurn = true;
                        enemyHand.isTurn = false;
                        _isPlayer1Playing = true;
                        break;
                    case Winner.ToBeDetermined:
                    default: throw new ArgumentOutOfRangeException();
                }
        }

        private async Task TurnPlayed() {
            if (playerHand.TurnEnded && enemyHand.TurnEnded) Endgame();

            if (_isPlayer1Playing) {
                Debug.Log("ENEMY TURN");
                enemyHand.isTurn = true;
                await EnemyTurn();
            }
            else {
                Debug.Log("PLAYER TURN");
                playerHand.isTurn = true;
            }

            _isPlayer1Playing = !_isPlayer1Playing;
        }

        private async Task EnemyTurn() {
            const int threshold = 5;
            var playerPower = playerLines.GetCombatPowers();
            var enemyPower = enemyLines.GetCombatPowers();

            if (enemyHand.CardCount > threshold ||
                (Compare(enemyPower, playerPower) <= playerPower.Length / 2f && enemyHand.CardCount > 0)) {
                var prng = new System.Random();
                await enemyHand.PlayCard(prng.Next(enemyHand.CardCount));
            }
            else enemyHand.EndTurn();
        }

        private void Endgame() {
            var playerPower = playerLines.GetCombatPowers();
            var enemyPower = enemyLines.GetCombatPowers();

            var playerScore = Compare(playerPower, enemyPower);
            var enemyScore = Compare(enemyPower, playerPower);

            if (playerScore > enemyScore) _scores[_round - 1] = Winner.Player1;
            else if (playerScore < enemyScore) _scores[_round - 1] = Winner.Player2;
            else _scores[_round - 1] = Winner.Tie;

            _round++;

            if (_round > 3) StartCoroutine(FinishGame());
            else SetupRound();
        }

        private static int Compare(IEnumerable<int> arraySource, IReadOnlyList<int> arrayTarget) { return arraySource.Where((t, i) => t > arrayTarget[i]).Count(); }

        private IEnumerator FinishGame() {
            resultScreen.SetActive(true);
            var p1Wins = _scores.Count(s => s == Winner.Player1);
            var p2Wins = _scores.Count(s => s == Winner.Player2);

            if (p1Wins > p2Wins) resultText.text = "You win !";
            else if (p1Wins < p2Wins) resultText.text = "You lose...";
            else resultText.text = "It's a tie !";

            yield return new WaitForSeconds(10);

            BackToMainMenu();
        }

        private void BackToMainMenu() { StartCoroutine(LoadMainMenu()); }

        private static IEnumerator LoadMainMenu() {
            yield return new WaitForSeconds(3);

            var main = SceneManager.LoadSceneAsync("Scenes/MainMenu", LoadSceneMode.Additive);
            yield return new WaitUntil(() => main.isDone);
            SceneManager.UnloadSceneAsync("Scenes/GameScene");
        }
    }
}

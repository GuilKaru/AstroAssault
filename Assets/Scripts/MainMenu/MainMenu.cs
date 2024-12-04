using Boom;
using MainMenu;
using TMPro;
using UnityEngine;

namespace AstroAssault
{
    public class MainMenu : MonoBehaviour
    {
        #region Private Variables
        [SerializeField]
        private GameObject _usernameMenu;
        [SerializeField]
        private GameObject _loginMenu;
        [SerializeField]
        private GameObject _mainMenu;
        [SerializeField]
        private GameObject _gameCanvas;
        [SerializeField]
        private GameObject _tutorialCanvas;

        [SerializeField]
        private PlayerData _playerData;

        [SerializeField]
        private TextMeshProUGUI _scoreTextAA;
        #endregion

        public void ChangeLoginMenu()
        {
            _loginMenu.SetActive(false);
            _usernameMenu.SetActive(true);

            //Logic for the username
        }

        public void ChangeUsernameMenu()
        {
            _usernameMenu.SetActive(false);
            _mainMenu.SetActive(true);

            //Username save
        }

        public void StartGame()
        {
            _mainMenu.SetActive(false);
            _gameCanvas.SetActive(true);

            //Start Game Logic
            GameManager.gameManager.StartGame();
        }

        public void NameSafe(string username)
        {
            _playerData.username = username;
            _usernameMenu.SetActive(false);
        }

        public void TutorialToggle(bool active)
        {
            _tutorialCanvas.SetActive(active);
        }

        public void ScoreUpdateAA()
        {
            var principal = UserUtil.GetPrincipal();
            EntityUtil.TryGetFieldAsText(principal, "score", "maxscore", out var outScore, "None");

            if (outScore == "None" || outScore == null)
            {
                ScoreSafeAA("0");
            }
            else
            {
                ScoreSafeAA(outScore);
            }

        }

        public void ScoreSafeAA(string score)
        {
            _playerData.maxScoreAA = int.Parse(score);
            //MainMenuGameManager.mMgameManager.boomLeadearboardPO.SetLeaderboardEntry("set_leaderboard_1", score, MainMenuGameManager.mMgameManager.playerData.username);
            _scoreTextAA.text = "Max Score: " + score;
        }
    }
}

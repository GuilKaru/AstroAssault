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
        }
    }
}

using UnityEngine;

namespace AstroAssault
{
    public class GameManager : MonoBehaviour
    {
        #region Singleton
        public static GameManager gameManager;

        private void Awake()
        {
            if (gameManager == null) gameManager = this;
        }
        #endregion

        #region Serialize Variables
        [SerializeField]
        public bool gameStarted = false;
        [SerializeField]
        public bool gamePaused = false;

        [SerializeField]
        public DifficultyManager difficultyManager;
        #endregion

        //Activate systems in game
        public void StartGame()
        {
            difficultyManager.SetSpawners();
            gameStarted = true;
        }

        //Deactivate everything in game
        public void EndGame()
        {
            //Stop stuff
            gameStarted = false;
        }
    }
}

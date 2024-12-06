using UnityEngine;

namespace AstroAssault
{
    public class GameManager : MonoBehaviour
    {
        //SingleTon
        #region Singleton
        public static GameManager gameManager;

        private void Awake()
        {
            if (gameManager == null) gameManager = this;

			PlayMusic();
		}
        #endregion

        //Serialize Fields
        #region Serialize Variables

        [SerializeField]
        public bool gameStarted = false;
        [SerializeField]
        public bool gamePaused = false;

        [SerializeField]
        public DifficultyManager difficultyManager;
		[SerializeField]
		public ScoreManager scoreManager;
		[SerializeField]
		public PlayerHealth playerHealth;
		
        [SerializeField]
        private GameObject _pauseMenuUI;
		[SerializeField]
		private GameObject _gameOverMenuUI;

		[SerializeField]
		private AudioSource _backgroundMusic;
		
		[SerializeField]
		public MainMenu mainMenu;
		
		[SerializeField]
		private BoomScore _boomScore;
		[SerializeField]
		private BoomGameOver _boomGameOver;
		#endregion

		//Activate systems in game
		#region Activate systems in game

		public void StartGame()
        {
            difficultyManager.SetSpawners();
            gameStarted = true;
        }

        //Deactivate everything in game
        private void EndGame()
        {
            //Stop stuff
            gameStarted = false;
        }
		#endregion

        //Pause Menu Logic
        #region Pause Menu Logic
        public void PauseGame(bool isPaused)
        {
            _pauseMenuUI.SetActive(isPaused);
            
            if (isPaused)
            {
                EndGame();
            }
            else
            {
                StartGame();
            }

        }


        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PauseGame(!gamePaused);
            }
        }
        #endregion

        //Restart Logic
        #region Restart Logic

        public void RestartGame()
        {
            ResetLogic();

			_gameOverMenuUI.SetActive(false);

		   // Reset any other game variables
		   gameStarted = true;

        }

        private void ClearEnemies()
		{
			// Assuming the parent GameObject for enemies is called "EnemyParent"
			GameObject enemyParent = GameObject.Find("EnemyParent");
			if (enemyParent != null)
			{
				foreach (Transform child in enemyParent.transform)
				{
					Destroy(child.gameObject); // Destroy each child (enemy)
				}
			}
		}

		private void ClearBuffs()
		{
			// Assuming the parent GameObject for enemies is called "EnemyParent"
			GameObject enemyParent = GameObject.Find("BuffParent");
			if (enemyParent != null)
			{
				foreach (Transform child in enemyParent.transform)
				{
					Destroy(child.gameObject); // Destroy each child (enemy)
				}
			}
		}

		private void ClearBullets()
		{
			// Assuming the parent GameObject for enemies is called "EnemyParent"
			GameObject enemyParent = GameObject.Find("BulletParent");
			if (enemyParent != null)
			{
				foreach (Transform child in enemyParent.transform)
				{
					Destroy(child.gameObject); // Destroy each child (enemy)
				}
			}
		}

		private void ResetLogic()
		{
			if (playerHealth != null)
			{
				playerHealth.SetPlayerHealth(playerHealth.maxHearts); // Reset to max health
				playerHealth.ResetDeathStatus(); // Clear death status
				playerHealth.ResetPosition();
			}

			// Reset the score
			if (scoreManager != null)
			{
				scoreManager.scoreCount = 0;
				scoreManager.ScoreUpdate();
			}

			// Reset difficulty
			if (difficultyManager != null)
			{
				difficultyManager.ResetDifficulty();
			}

			// Clear all enemies from the scene
			ClearEnemies();
			ClearBuffs();
			ClearBullets();
		}

		#endregion

		//GameOver Logic
		#region GameOver Logic

		public void GameOver()
		{
			EndGame();
			_gameOverMenuUI.SetActive(true);
			_boomScore.ActionButtonClickHandler(scoreManager.scoreCount);
			//_boomGameOver.ActionButtonClickHandler("match_outcome_finish");
		}

		public void BackToMainMenu()
		{
			ResetLogic();
			
			_pauseMenuUI.SetActive(false);
			_gameOverMenuUI.SetActive(false);
			mainMenu._mainMenu.SetActive(true);
			mainMenu._gameCanvas.SetActive(false);
		}
		#endregion

		// Music Control Logic
		#region Music Control

		private void PlayMusic()
		{
			if (_backgroundMusic != null && !_backgroundMusic.isPlaying)
			{
				_backgroundMusic.Play();
			}
		}

		private void PauseMusic()
		{
			if (_backgroundMusic != null && _backgroundMusic.isPlaying)
			{
				_backgroundMusic.Pause();
			}
		}

		private void ResumeMusic()
		{
			if (_backgroundMusic != null && !_backgroundMusic.isPlaying)
			{
				_backgroundMusic.UnPause();
			}
		}

		private void StopMusic()
		{
			if (_backgroundMusic != null)
			{
				_backgroundMusic.Stop();
			}
		}

		#endregion
	}
}

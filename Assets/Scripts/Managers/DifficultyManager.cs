using UnityEngine;

namespace AstroAssault
{
	public class DifficultyManager : MonoBehaviour
	{
		//Serialize Fields
		#region Serialize Fields
		[Header("Enemy References")]
		[SerializeField]
		private EnemySpawner _enemySpawner; // Reference to the spawner
		[SerializeField]
		private GameObject[] _enemyPrefabs; // Full list of enemies
		[SerializeField]
		private DrillSpawner _drillSpawner;
		[SerializeField]
		private GameObject[] drillPrefabs;

		[Header("Score References")]
		[SerializeField]
		private ScoreManager _scoreManager;
		[SerializeField]
		private int[] _scoreMilestones; // Score thresholds for unlocking enemies
		[SerializeField]
		private int drillUnlockScore = 1500;
		#endregion

		//Private Variables
		#region Private Variables
		private int _currentLevel = 0;
		private bool drillsUnlocked = false;

		#endregion

		//Initialization
		#region Initialization

		//private void Start()
		//{
		//	// Initialize the spawner with the first enemy only (Asteroid)
		//	_enemySpawner.SetSpawnableEnemies(new GameObject[] { _enemyPrefabs[0] });
		//	_drillSpawner.SetSpawnableDrills(null);
		//}

		public void SetSpawners()
		{
            // Initialize the spawner with the first enemy only (Asteroid)
            _enemySpawner.SetSpawnableEnemies(new GameObject[] { _enemyPrefabs[0] });
            _drillSpawner.SetSpawnableDrills(null);
        }

		private void Update()
		{
            if (!GameManager.gameManager.gameStarted) return;
            if (GameManager.gameManager.gamePaused) return;

            // Check score every frame
            int currentScore = _scoreManager.GetScore();

			// Check if we should unlock a new enemy type
			if (_currentLevel < _scoreMilestones.Length && currentScore >= _scoreMilestones[_currentLevel])
			{		
				UnlockNewEnemy();
			}

			if (!drillsUnlocked && currentScore >= drillUnlockScore)
			{
				UnlockDrills();
			}
		}
		#endregion

		//Enemy Progression
		#region Enemy Progression

		private void UnlockNewEnemy()
		{
			// Unlock the next enemy type
			_currentLevel++;

			// Build the updated spawnable enemy list
			GameObject[] unlockedEnemies = new GameObject[_currentLevel + 1];
			for (int i = 0; i <= _currentLevel; i++)
			{
				unlockedEnemies[i] = _enemyPrefabs[i];
			}

			// Update the spawner
			_enemySpawner.SetSpawnableEnemies(unlockedEnemies);
			Debug.Log($"Unlocked new enemy: {_enemyPrefabs[_currentLevel].name}");
		}

		private void UnlockDrills()
		{
			drillsUnlocked = true;

			// Add all drill prefabs to the drill spawner
			_drillSpawner.SetSpawnableDrills(drillPrefabs);

			Debug.Log("Drills unlocked!");
		}
		#endregion

		//Difficulty Reset
		#region Difficulty Reset
		public void ResetDifficulty()
		{
			_currentLevel = 0;
			drillsUnlocked = false;
			SetSpawners(); // Reset spawners to initial state
		}
		#endregion
	}
}


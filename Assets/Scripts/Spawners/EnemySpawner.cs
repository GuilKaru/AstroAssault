using UnityEngine;

namespace AstroAssault
{
	public class EnemySpawner : MonoBehaviour
	{
		//Serialize Fields
		#region Serialize Fields
		[SerializeField]
		private Transform _enemyParent;
		[SerializeField]
		private float _spawnInterval = 2f; // Time between spawns
		[SerializeField]
		private Vector2 _spawnAreaSize = new Vector2(5f, 5f); // Width and height of the spawn area
		[SerializeField]
		private float _spawnRotation = 90f;
		#endregion

		//Private Variables
		#region Private Variables
		private GameObject[] _currentEnemies;
		#endregion

		//Initialization
		#region Initialization
		private void Start()
		{
			InvokeRepeating(nameof(SpawnEnemy), 0f, _spawnInterval);
		}
		#endregion

		//Spawn Logic
		#region Spawn Logic


		public void SetSpawnableEnemies(GameObject[] newEnemyPrefabs)
		{
			_currentEnemies = newEnemyPrefabs;
		}

		private void SpawnEnemy()
		{
			if (!GameManager.gameManager.gameStarted) return;
			if (_currentEnemies == null || _currentEnemies.Length == 0) return;

			// Pick a random enemy prefab
			int randomIndex = Random.Range(0, _currentEnemies.Length);
			GameObject enemyToSpawn = _currentEnemies[randomIndex];

			// Randomize position within spawn area
			Vector2 spawnPosition = new Vector2(
				Random.Range(-_spawnAreaSize.x / 2f, _spawnAreaSize.x / 2f),
				Random.Range(-_spawnAreaSize.y / 2f, _spawnAreaSize.y / 2f)
			);

			// Adjust position relative to the spawner
			Vector3 worldPosition = transform.position + (Vector3)spawnPosition;

			// Spawn the enemy with the correct rotation
			Quaternion enemyRotation = Quaternion.Euler(0, 0, _spawnRotation);
			Instantiate(enemyToSpawn, worldPosition, enemyRotation, _enemyParent);
		}
		#endregion

		//Gizmos
		#region Gizmos
		private void OnDrawGizmos()
		{
			// Visualize the spawn area
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(transform.position, new Vector3(_spawnAreaSize.x, _spawnAreaSize.y, 0f));
		}
		#endregion
	}
}


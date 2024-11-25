using UnityEngine;

namespace AstroAssault
{
	public class DrillSpawner : MonoBehaviour
	{
		//Serialize Fields
		#region Serialize Fields

		[SerializeField]
		private float _spawnInterval = 3f; // Time between spawns
		[SerializeField]
		private Vector2 _spawnAreaSize = new Vector2(5f, 5f);
		[SerializeField]
		private float _spawnRotation = 90f; // Rotation for spawned drills
		[SerializeField]
		private Transform _drillParent;
		#endregion

		//Private Variables 
		#region Private Variables
		private GameObject[] _currentDrills; // Dynamically updated list of drill enemies
		#endregion

		//Initialization
		#region Initialization

		private void Start()
		{
			// Start spawning drills
			InvokeRepeating(nameof(SpawnDrill), 0f, _spawnInterval);
		}
		#endregion

		//Spawn Logic
		#region Spawn Logic

		public void SetSpawnableDrills(GameObject[] newDrillPrefabs)
		{
			_currentDrills = newDrillPrefabs; // Update the list of spawnable drills
		}

		private void SpawnDrill()
		{
			if (_currentDrills == null || _currentDrills.Length == 0) return;

			// Pick a random drill prefab from the current list
			int randomIndex = Random.Range(0, _currentDrills.Length);
			GameObject drillToSpawn = _currentDrills[randomIndex];

			// Randomize position within spawn area
			Vector2 spawnPosition = new Vector2(
				Random.Range(-_spawnAreaSize.x / 2f, _spawnAreaSize.x / 2f),
				Random.Range(-_spawnAreaSize.y / 2f, _spawnAreaSize.y / 2f)
			);

			// Convert to world position relative to the spawner
			Vector3 worldPosition = transform.position + (Vector3)spawnPosition;

			// Spawn the drill with the correct rotation
			Quaternion drillRotation = Quaternion.Euler(0, 0, _spawnRotation);
			Instantiate(drillToSpawn, worldPosition, drillRotation, _drillParent);
		}
		#endregion

		//Gizmos
		#region Gizmos

		private void OnDrawGizmos()
		{
			// Visualize the spawn area
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(transform.position, new Vector3(_spawnAreaSize.x, _spawnAreaSize.y, 0f));
		}
		#endregion
	}

}

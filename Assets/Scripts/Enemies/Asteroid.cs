using UnityEngine;

namespace AstroAssault
{
	public class Asteroid : MonoBehaviour
	{
		// Serialize Fields
		#region Serialize Fields
		[SerializeField]
		private float _moveSpeed = 3f;  // Speed of horizontal movement
		[SerializeField]
		private int _scoreValue = 50;  // Score given on destruction
		[SerializeField]
		private GameObject _smallerAsteroidPrefab;
		[SerializeField] private float _spreadAngle = 30f;
		#endregion

		// Private Variables
		#region Private Variables
		private ScoreManager _scoreManager;
		private PlayerHealth _playerHealth;
		private Transform _enemyParent;
		#endregion

		// Initialization
		#region Initialization
		private void Start()
		{
			// Locate the ScoreManager in the scene
			_scoreManager = FindObjectOfType<ScoreManager>();

			// Locate the PlayerHealth in the scene
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if (player != null)
			{
				_playerHealth = player.GetComponent<PlayerHealth>();
			}

			GameObject enemyParentObject = GameObject.Find("EnemyParent");
			if (enemyParentObject == null)
			{
				enemyParentObject = new GameObject("EnemyParent");
			}
			_enemyParent = enemyParentObject.transform;

			// Set this asteroid as a child of EnemyParent
			transform.SetParent(_enemyParent);
		}
		#endregion

		// Movement
		#region Movement
		private void Update()
		{
			if (!GameManager.gameManager.gameStarted) return;
			// Move the asteroid to the left
			transform.position += Vector3.left * _moveSpeed * Time.deltaTime;

			if (transform.position.x < -15f)  // Destroy when x is less than -10
			{
				Destroy(gameObject);
			}
		}
		#endregion

		// Collision 
		#region Collision
		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (collision.gameObject.CompareTag("PlayerBullet"))
			{
				// Add score to the ScoreManager
				if (_scoreManager != null)
				{
					_scoreManager.AddScore(_scoreValue);
				}

				// Spawn two smaller asteroids
				SpawnSmallerAsteroids();

				// Destroy this asteroid and the bullet
				Destroy(collision.gameObject);
				Destroy(gameObject);
			}

			if (collision.gameObject.CompareTag("Player"))
			{
				// Check if PlayerHealth is available and apply damage
				if (_playerHealth != null)
				{
					_playerHealth.Damage(1);
				}


				// Destroy the asteroid upon collision with the player
				Destroy(gameObject);
			}
		}

		private void SpawnSmallerAsteroids()
		{
			if (_smallerAsteroidPrefab != null)
			{
				// Define two different angles for the smaller asteroids
				for (int i = -1; i <= 1; i += 2) // -1 and 1 for left and right spread
				{
					float angle = i * _spreadAngle; // Calculate angle for spread
					Quaternion rotation = Quaternion.Euler(0, 0, angle) * transform.rotation;

					// Instantiate smaller asteroid
					GameObject smallerAsteroid = Instantiate(_smallerAsteroidPrefab, transform.position, rotation);

					if (_enemyParent != null)
					{
						smallerAsteroid.transform.SetParent(_enemyParent);
					}

					// Apply force in the direction based on rotation
					Rigidbody2D rb = smallerAsteroid.GetComponent<Rigidbody2D>();
					if (rb != null)
					{
						Vector2 direction = rotation * Vector2.up; // Adjust direction based on rotation
						rb.AddForce(direction * _moveSpeed, ForceMode2D.Impulse);
					}
				}
			}
		}



		#endregion
	}
}
	


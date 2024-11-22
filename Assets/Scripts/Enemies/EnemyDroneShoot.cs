using UnityEngine;

namespace AstroAssault
{
	public class EnemyDroneShoot : MonoBehaviour
	{
		//Serialize Fields
		#region Serialize Fields
		[Header("Shooting Settings")]
		[SerializeField]
		private GameObject _enemyBulletPrefab; // Bullet prefab to instantiate
		[SerializeField]
		private Transform _shootingPoint; // Point from where bullets are fired
		[SerializeField]
		private float _shootInterval = 2f; // Time interval between shots

		[Header("Shooting Toggle")]
		[SerializeField]
		private bool _canShoot = true;

		[Header("Movement Settings")]
		[SerializeField]
		private float _verticalSpeed = 2f; // Speed of up-and-down movement
		[SerializeField]
		private float _verticalAmplitude = 1f; // Amplitude of the sine wave
		[SerializeField]
		private float _horizontalSpeed = 2f; // Speed of leftward movement
		[SerializeField]
		private float _destroyXThreshold = -15f; // X threshold to destroy the drone

		[Header("Points")]
		[SerializeField]
		private int _scorePoints = 100;
		#endregion

		//Private variables
		#region Private variables
		private GameObject _bulletParent;
		private float _shootTimer; 
		private Vector3 _initialPosition; 
		private float _timeCounter;
		private ScoreManager _scoreManager;
		private PlayerHealth _playerHealth;
		#endregion

		//Initialization
		#region Initialization
		private void Start()
		{
			_shootTimer = _shootInterval; // Initialize timer
			_initialPosition = transform.position;
			FindBulletParent();

			// Locate the ScoreManager in the scene
			_scoreManager = FindObjectOfType<ScoreManager>();

			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if (player != null)
			{
				_playerHealth = player.GetComponent<PlayerHealth>();

			}
		}

		private void Update()
		{
			HandleShooting();
			HandleMovement();
		}
		#endregion

		//Drone Shooting
		#region Drone Shooting
		private void HandleShooting()
		{
			if (!_canShoot) return;

			_shootTimer -= Time.deltaTime;
			if (_shootTimer < 0)
			{
				Shoot();
				_shootTimer = _shootInterval;
			}
		}

		private void Shoot()
		{
			if (_enemyBulletPrefab != null && _shootingPoint != null)
			{
				GameObject bullet = Instantiate(_enemyBulletPrefab, _shootingPoint.position, Quaternion.identity);

				// Parent the bullet to BulletParent if it exists
				if (_bulletParent != null)
				{
					bullet.transform.SetParent(_bulletParent.transform);
				}

				// Set bullet's target direction
				EnemyBulletController bulletController = bullet.GetComponent<EnemyBulletController>();

				if (bulletController != null)
				{
					Vector3 targetPosition = _shootingPoint.position + _shootingPoint.up * 10f;
					bulletController.InitializeMovement(targetPosition);
				}
			}
		}
		#endregion

		//Movement
		#region Movement
		private void HandleMovement()
		{
			// Calculate vertical sine wave movement
			_timeCounter += Time.deltaTime * _verticalSpeed;
			float verticalOffset = Mathf.Sin(_timeCounter) * _verticalAmplitude;

			// Move the drone
			transform.position = new Vector3(
				transform.position.x - _horizontalSpeed * Time.deltaTime, // Move left
				_initialPosition.y + verticalOffset,                     // Move up and down
				transform.position.z                                     // Keep Z constant
			);

			// Destroy the drone if it goes off-screen
			if (transform.position.x <= _destroyXThreshold)
			{
				Destroy(gameObject);
			}
		}


		#endregion

		//Collision
		#region Collision

		private void OnCollisionEnter2D(Collision2D collision)
		{
			// Check for collision with player's bullets
			if (collision.gameObject.CompareTag("PlayerBullet"))
			{
				// Add score to the ScoreManager
				if (_scoreManager != null)
				{
					_scoreManager.AddScore(_scorePoints);
				}

				// Destroy this enemy and the bullet
				Destroy(collision.gameObject);
				Destroy(gameObject);
			}

			if (collision.gameObject.CompareTag("Player"))
			{
				_playerHealth.Damage(1);
				Destroy(gameObject);
			}
		}
		#endregion

		//Bullet Parent
		#region Bullet Parent
		private void FindBulletParent()
		{
			_bulletParent = GameObject.Find("BulletParent");

			if (_bulletParent == null)
			{
				Debug.LogWarning("BulletParent GameObject not found. Bullets will not be parented.");
			}
		}
		#endregion
	}
}


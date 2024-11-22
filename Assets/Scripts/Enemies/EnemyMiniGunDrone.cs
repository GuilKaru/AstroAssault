using UnityEngine;

namespace AstroAssault
{
	public class EnemyMiniGunDrone : MonoBehaviour
	{
		//Serialize Fields
		#region Serialize Fields
		[Header("Shooting Settings")]
		[SerializeField]
		private GameObject _bulletPrefab; // Bullet prefab to instantiate
		[SerializeField]
		private Transform _shootingPoint; // Point from where bullets are fired
		[SerializeField]
		private float _shootInterval = 0.5f; // Time interval between shots

		[Header("Movement Settings")]
		[SerializeField]
		private float _moveSpeed = 2f; // Speed of straight-line movement
		[SerializeField]
		private float _moveDuration = 2f; // Time duration for movement before rotation starts

		[Header("Points")]
		[SerializeField]
		private int _scorePoints = 100;

		[Header("Rotation Settings")]
		[SerializeField]
		private float _rotationSpeed = 100f; // Speed of rotation while shooting
		#endregion

		//Private Variables
		#region Private Variables

		private float _shootTimer; // Timer to track shooting interval
		private float _moveTimer; // Timer to track movement duration
		private bool _isRotating = false; // Flag to indicate if the drone is in the rotating phase
		private GameObject _bulletParent; // Reference to the BulletParent GameObject

		private ScoreManager _scoreManager;
		private PlayerHealth _playerHealth;
		#endregion

		//Initialization
		#region Initialization
		private void Start()
		{
			_moveTimer = _moveDuration; // Initialize movement timer
			_shootTimer = _shootInterval; // Initialize shooting timer
			FindBulletParent(); // Dynamically find the BulletParent GameObject

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
			if (_isRotating)
			{
				HandleRotationAndShooting();
			}
			else
			{
				HandleMovement();
			}
		}
		#endregion

		//Movement
		#region Movement
		private void HandleMovement()
		{
			// Move in a straight line
			transform.position += Vector3.left * _moveSpeed * Time.deltaTime;

			// Decrease movement timer
			_moveTimer -= Time.deltaTime;

			// Switch to rotating phase after the movement duration
			if (_moveTimer <= 0f)
			{
				_isRotating = true;
			}
		}
		#endregion

		//Rotation and Shoot
		#region Rotation and Shoot
		private void HandleRotationAndShooting()
		{
			// Rotate the drone
			transform.Rotate(Vector3.forward, _rotationSpeed * Time.deltaTime);

			// Handle continuous shooting
			_shootTimer -= Time.deltaTime;

			if (_shootTimer <= 0f)
			{
				Shoot();
				_shootTimer = _shootInterval; // Reset timer
			}
		}

		private void Shoot()
		{
			if (_bulletPrefab != null && _shootingPoint != null)
			{
				// Instantiate bullet
				GameObject bullet = Instantiate(_bulletPrefab, _shootingPoint.position, Quaternion.identity);

				// Parent the bullet to BulletParent if it exists
				if (_bulletParent != null)
				{
					bullet.transform.SetParent(_bulletParent.transform);
				}

				// Initialize the bullet movement
				EnemyBulletController bulletController = bullet.GetComponent<EnemyBulletController>();
				if (bulletController != null)
				{
					Vector3 shootDirection = _shootingPoint.up; // Adjust for 2D or other axes
					bulletController.InitializeMovement(shootDirection);
				}
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


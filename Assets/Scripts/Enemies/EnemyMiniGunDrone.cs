using UnityEngine;
using System.Collections;

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
		[SerializeField]
		SpriteRenderer _rotatingSprite;

		[Header("Animation")]
		[SerializeField]
		private Animator _animator;

		[Header("Enemy Audio")]
		[SerializeField]
		private AudioSource _shootAudioSource;
		[SerializeField]
		private AudioClip[] _shootClips;
		[SerializeField]
		private AudioSource _hitAudioSource;
		[SerializeField]
		private AudioClip[] _enemyHitClip;


		[Header("Buffs")]
		[SerializeField]
		private GameObject _buff1Prefab; // First buff prefab
		[SerializeField]
		private GameObject _buff2Prefab; // Second buff prefab
		[SerializeField, Range(0f, 1f)]
		private float _buffSpawnChance = 0.35f; // 35% chance

		#endregion

		//Private Variables
		#region Private Variables

		private float _shootTimer; // Timer to track shooting interval
		private float _moveTimer; // Timer to track movement duration
		private bool _isRotating = false; // Flag to indicate if the drone is in the rotating phase
		private GameObject _bulletParent; // Reference to the BulletParent GameObject

		private ScoreManager _scoreManager;
		private PlayerHealth _playerHealth;

		//Animations
		private string _currentState;
		private string _idleAnim = "EnemyMiniGun_Idle";
		private string _deathAnim = "Enemy_Death";
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
			_animator = GetComponent<Animator>();

			_hitAudioSource = GetComponentInChildren<AudioSource>();
			_shootAudioSource = GetComponentInChildren<AudioSource>();


			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if (player != null)
			{
				_playerHealth = player.GetComponent<PlayerHealth>();

			}
			ChangeAnimationState(_idleAnim);
		}

		private void Update()
		{
			if (!GameManager.gameManager.gameStarted) return;
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
				// Create a base rotation for the bullet (aligned to global "up" or intended direction)
				Quaternion baseRotation = Quaternion.Euler(0, 0, 0); // Adjust this to match your "default forward" direction.

				// Combine base rotation with the shooting point's local rotation
				Quaternion finalRotation = baseRotation * _shootingPoint.rotation;

				// Instantiate the bullet
				GameObject bullet = Instantiate(_bulletPrefab, _shootingPoint.position, finalRotation);
				PlayAudioShootClip(0);

				// Parent the bullet to BulletParent if it exists
				if (_bulletParent != null)
				{
					bullet.transform.SetParent(_bulletParent.transform);
				}

				// Initialize the bullet movement
				EnemyBulletController bulletController = bullet.GetComponent<EnemyBulletController>();
				if (bulletController != null)
				{
					// Use the final rotation to determine the movement direction
					Vector3 shootDirection = finalRotation * Vector3.left; // Adjust if your bullet's "forward" is different
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
				ChangeAnimationState(_deathAnim);
				PlayAudioEnemyHitClip(0);
				GetComponent<Collider2D>().enabled = false;
				_rotatingSprite.GetComponent<SpriteRenderer>().enabled = false;
				StartCoroutine(DestroyAfterDelay(1f));
			}

			TrySpawnBuff();

			if (collision.gameObject.CompareTag("Player"))
			{
				_playerHealth.Damage(1);
				ChangeAnimationState(_deathAnim);
				GetComponent<Collider2D>().enabled = false;
				PlayAudioEnemyHitClip(0);
				_rotatingSprite.GetComponent<SpriteRenderer>().enabled = false;
				StartCoroutine(DestroyAfterDelay(0.70f));
			}
		}
		#endregion

		//Spawn Buff
		#region Spawn Buff
		private void TrySpawnBuff()
		{
			// Roll for spawn chance
			if (Random.value <= _buffSpawnChance)
			{
				// Determine which buff to spawn
				GameObject selectedBuff = Random.value < 0.5f ? _buff1Prefab : _buff2Prefab;

				// Locate BuffParent
				GameObject buffParent = GameObject.Find("BuffParent");
				if (buffParent == null)
				{
					Debug.LogWarning("BuffParent not found! Buffs will spawn in the root of the hierarchy.");
				}

				// Spawn the buff at the drone's position, as a child of BuffParent
				GameObject spawnedBuff = Instantiate(selectedBuff, transform.position, Quaternion.identity);
				if (buffParent != null)
				{
					spawnedBuff.transform.SetParent(buffParent.transform);
				}
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

		//Animations
		#region Animations
		private void ChangeAnimationState(string newState)
		{
			// Avoid transitioning to the same animation
			if (_currentState == newState) return;

			// Play the new animation
			_animator.Play(newState);

			// Update the current state
			_currentState = newState;

		}

		private IEnumerator DestroyAfterDelay(float delay)
		{
			yield return new WaitForSeconds(delay);
			Destroy(gameObject);
		}
		#endregion

		//Enemy Audio
		#region Enemy Audio

		private void PlayAudioShootClip(int clipIndex)
		{
			if (clipIndex >= 0 && clipIndex < _shootClips.Length)
			{
				_shootAudioSource.clip = _shootClips[clipIndex];
				_shootAudioSource.Play();
			}
		}

		public void PlayAudioEnemyHitClip(int clipIndex)
		{
			if (clipIndex >= 0 && clipIndex < _enemyHitClip.Length)
			{
				_hitAudioSource.clip = _enemyHitClip[clipIndex];
				_hitAudioSource.Play();
			}
		}
		#endregion
	}
}


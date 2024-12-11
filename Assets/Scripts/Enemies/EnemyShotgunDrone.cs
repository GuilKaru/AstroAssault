using UnityEngine;
using System.Collections;

namespace AstroAssault
{
	public class EnemyShotgunDrone : MonoBehaviour
	{
		// Serialize Fields
		#region Serialize Fields
		[Header("Shooting Settings")]
		[SerializeField]
		private GameObject _enemyBulletPrefab; // Bullet prefab to instantiate
		[SerializeField]
		private Transform _shootingPoint; // Point from where bullets are fired
		[SerializeField]
		private float _shootInterval = 2f; // Time interval between shots
		[SerializeField]
		private float _spreadAngle = 15f; // Angle difference for shotgun spread
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

		// Private Variables
		#region Private Variables
		private GameObject _bulletParent; // Parent GameObject for bullets
		private float _shootTimer; // Timer for shooting
		private Vector3 _initialPosition; // Starting position of the drone
		private float _timeCounter; // Time counter for sine wave movement
		private ScoreManager _scoreManager; // Reference to ScoreManager
		private PlayerHealth _playerHealth; // Reference to PlayerHealth

		//Animations
		private string _currentState;
		private string _idleAnim = "EnemyShotgun_Idle";
		private string _deathAnim = "Enemy_Death";

		#endregion

		// Initialization
		#region Initialization
		private void Start()
		{
			_shootTimer = _shootInterval; // Initialize timer
			_initialPosition = transform.position;

			// Locate BulletParent, ScoreManager, and PlayerHealth
			FindBulletParent();
			_scoreManager = FindObjectOfType<ScoreManager>();
			_animator = GetComponent<Animator>();
			_hitAudioSource = GetComponentInChildren<AudioSource>();
			_shootAudioSource = GetComponentInChildren<AudioSource>();

			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if (player != null)
			{
				_playerHealth = player.GetComponent<PlayerHealth>();
			}
		}

		private void Update()
		{
			if (!GameManager.gameManager.gameStarted) return;
			if (GameManager.gameManager.gamePaused) return;
			
			HandleShooting();
			HandleMovement();
		}
		#endregion

		// Drone Shooting
		#region Drone Shooting
		private void HandleShooting()
		{
			if (!_canShoot) return;

			_shootTimer -= Time.deltaTime;
			if (_shootTimer <= 0)
			{
				ShootShotgun();
				_shootTimer = _shootInterval;
			}
		}

		private void ShootShotgun()
		{
			if (_enemyBulletPrefab != null && _shootingPoint != null)
			{

				// Base rotation (set to face directly upwards in world space)
				Quaternion baseRotation = Quaternion.Euler(0, 0, 0); // Adjust as needed to your "forward" direction

				// Shoot 3 bullets in a spread
				for (int i = -1; i <= 1; i++) // -1, 0, 1 for left, center, and right bullets
				{
					float angle = i * _spreadAngle; // Calculate spread angle
					Quaternion spreadRotation = Quaternion.Euler(0, 0, angle); // Spread rotation

					// Combine the base rotation with the spread rotation
					Quaternion finalRotation = baseRotation * spreadRotation;

					// Instantiate the bullet
					GameObject bullet = Instantiate(_enemyBulletPrefab, _shootingPoint.position, finalRotation);
					PlayAudioShootClip(0);

					// Parent the bullet to BulletParent if it exists
					if (_bulletParent != null)
					{
						bullet.transform.SetParent(_bulletParent.transform);
					}

					// Initialize the bullet's movement direction
					EnemyBulletController bulletController = bullet.GetComponent<EnemyBulletController>();
					if (bulletController != null)
					{
						// Use the final rotation to determine the movement direction
						Vector3 direction = finalRotation * Vector3.left; // Adjust if "up" isn't your intended direction
						bulletController.InitializeMovement(direction);
					}

				}
			}
		}
		#endregion

		// Drone Movement
		#region Drone Movement
		private void HandleMovement()
		{
			ChangeAnimationState(_idleAnim);
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

		// Collision and Scoring
		#region Collision and Scoring
		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (collision.gameObject.CompareTag("PlayerBullet"))
			{
				// Add score to the ScoreManager
				if (_scoreManager != null)
				{
					_scoreManager.AddScore(_scorePoints);
				}

				TrySpawnBuff();


				// Destroy this enemy and the player's bullet
				Destroy(collision.gameObject);

				ChangeAnimationState(_deathAnim);
				PlayAudioEnemyHitClip(0);

				GetComponent<Collider2D>().enabled = false;
				StartCoroutine(DestroyAfterDelay(1f));
			}

			if (collision.gameObject.CompareTag("Player"))
			{
				// Damage the player
				if (_playerHealth != null)
				{
					_playerHealth.Damage(1);
				}

				ChangeAnimationState(_deathAnim);
				PlayAudioEnemyHitClip(0);

				GetComponent<Collider2D>().enabled = false;
				StartCoroutine(DestroyAfterDelay(1f));
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

		//Animation
		#region Animation

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

		// Bullet Parent
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

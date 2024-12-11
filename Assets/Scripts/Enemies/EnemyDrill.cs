using UnityEngine;
using System.Collections;

namespace AstroAssault
{
	public class EnemyDrill : MonoBehaviour
	{
		//Serialize Fields
		#region Serialize Fields
		[Header("Movement Settings")]
		[SerializeField]
		private float _slowSpeed = 1f; // Speed for the initial slow descent
		[SerializeField]
		private float _fastSpeed = 10f; // Speed for the rapid descent
		[SerializeField]
		private float _waitTime = 2f; // Time to wait before accelerating
		[SerializeField]
		private float _destroyYThreshold = -10f; // Y threshold to destroy the drill

		[Header("Points")]
		[SerializeField]
		private int _scorePoints = 100;

		[Header("Animation")]
		[SerializeField]
		private Animator _animator;

		[Header("Enemy Audio")]
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
		private float _waitTimer; // Timer to track the waiting period
		private bool _isWaiting = false; // Flag to indicate the waiting phase
		private bool _isFastDescent = false; // Flag to indicate the fast descent phase
		private float _targetY;

		private ScoreManager _scoreManager;
		private PlayerHealth _playerHealth;

		//Animations
		private string _currentState;
		private string _idleAnim = "EnemyDrill_Idle";
		private string _deathAnim = "Enemy_Death";
		#endregion

		//Initialization
		#region Initialization
		private void Start()
		{
			_waitTimer = _waitTime; // Initialize the wait timer
			_targetY = transform.position.y - 2f;

			// Locate the ScoreManager in the scene
			_scoreManager = FindObjectOfType<ScoreManager>();

			_hitAudioSource = GetComponent<AudioSource>();
			_animator = GetComponent<Animator>();

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
			
			if (_isFastDescent)
			{
				ChangeAnimationState(_idleAnim);
				PerformFastDescent();
			}
			else if (_isWaiting)
			{
				HandleWaiting();
			}
			else
			{
				PerformSlowDescent();
			}
		}
		#endregion

		//Movement
		#region Movement 
		private void PerformSlowDescent()
		{
			transform.position += Vector3.down * _slowSpeed * Time.deltaTime; // Transition to waiting phase when reaching the target Y position

			if (transform.position.y <= _targetY)
			{ 
				_isWaiting = true;
			}

		}

		private void HandleWaiting()
		{
			_waitTimer -= Time.deltaTime;

			if (_waitTimer <= 0f)
			{
				_isFastDescent = true;
			}
		}

		private void PerformFastDescent()
		{
			// Move down rapidly
			transform.position += Vector3.down * _fastSpeed * Time.deltaTime;

			// Destroy the drill if it goes out of bounds
			if (transform.position.y <= _destroyYThreshold)
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

				TrySpawnBuff();


				// Destroy this enemy and the bullet
				Destroy(collision.gameObject);

				ChangeAnimationState(_deathAnim);
				PlayAudioEnemyHitClip(0);
				GetComponent<Collider2D>().enabled = false;
				StartCoroutine(DestroyAfterDelay(1f));
			}

			if (collision.gameObject.CompareTag("Player"))
			{
				_playerHealth.Damage(1);
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


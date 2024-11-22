using UnityEngine;

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
		#endregion

		//Private Variables
		#region Private Variables
		private float _waitTimer; // Timer to track the waiting period
		private bool _isWaiting = false; // Flag to indicate the waiting phase
		private bool _isFastDescent = false; // Flag to indicate the fast descent phase
		private float _targetY;

		private ScoreManager _scoreManager;
		private PlayerHealth _playerHealth;
		#endregion

		//Initialization
		#region Initialization
		private void Start()
		{
			_waitTimer = _waitTime; // Initialize the wait timer
			_targetY = transform.position.y - 2f;

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
			if (_isFastDescent)
			{
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
	}
}


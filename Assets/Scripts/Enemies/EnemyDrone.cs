using UnityEngine;

namespace AstroAssault
{
	public class EnemyDrone : MonoBehaviour
	{
		//Serialize Fields
		#region Serialize Fields
		[Header("Movement")]
		[SerializeField]
		private float _delayBeforeMove = 2f;
		[SerializeField]
		private float _moveSpeed = 5f;
		[SerializeField]
		private float _destroyXPosition = -10f;

		[Header("Points")]
		[SerializeField]
		private int _scorePoints = 100;
		#endregion

		//Private Variables
		#region Private Varibles
		private bool _shouldMove = false;
		private ScoreManager _scoreManager;
		private PlayerHealth _playerHealth;
		private Vector3 _movementDirection;
		
		#endregion

		//Initialization
		#region Initialization

		private void Start()
		{
			// Locate the ScoreManager in the scene
			_scoreManager = FindObjectOfType<ScoreManager>();

			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if (player != null)
			{
				_playerHealth = player.GetComponent<PlayerHealth>();
			
			}
			

			// Start the delay and set the target position
			Invoke(nameof(InitializeMovement), _delayBeforeMove);
		}

		private void Update()
		{
			if (_shouldMove)
			{
				// Move in the calculated direction
				transform.position += _movementDirection * _moveSpeed * Time.deltaTime;

				// Destroy the drone if it passes the X position threshold
				if (transform.position.x <= _destroyXPosition)
				{
					Destroy(gameObject);
				}
			}
		}
		#endregion

		//Movement
		#region Movement

		private void InitializeMovement()
		{
			// Assuming the player object has the "Player" tag
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if (player != null)
			{
				_movementDirection = (player.transform.position - transform.position).normalized;
				_shouldMove = true;
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


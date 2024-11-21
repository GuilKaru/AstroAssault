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
		#endregion

		// Private Variables
		#region Private Variables
		private ScoreManager _scoreManager;
		private PlayerHealth _playerHealth;
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
		
		}
		#endregion

		// Movement
		#region Movement
		private void Update()
		{
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
		#endregion
	}

}

using UnityEngine;

namespace AstroAssault
{
	public class EnemyBulletController : MonoBehaviour
	{
		//Serialize Fields
		#region Serialize Fields
		[Header("Bullet Settings")]
		[SerializeField]
		private float _lifetime = 5f; // Bullet lifetime before auto-destruction
		[SerializeField]
		private float _speed = 10f;

		#endregion

		//Private Variables
		#region Private Variables
		private Vector3 _movementDirection;
		private float _lifetimeTimer; // Timer to track bullet's lifetime
		#endregion

		//Initialization
		#region Initialization
		private void Start()
		{
			_lifetimeTimer = _lifetime; // Initialize the timer
		}

		private void Update()
		{
			if (!GameManager.gameManager.gameStarted) return;
			HandleLifetime();
			MoveBullet();
		}
		#endregion

		// Movement
		#region Movement
		public void InitializeMovement(Vector3 direction)
		{
			_movementDirection = direction.normalized; // Normalize to ensure consistent speed
		}

		private void MoveBullet()
		{
			transform.position += _movementDirection * _speed * Time.deltaTime; // Move the bullet
		}
		#endregion

		//Collision
		#region Collision

		private void OnCollisionEnter2D(Collision2D collision)
		{
		
			if (collision.gameObject.CompareTag("Player")) // Assuming player has the "Player" tag
			{
				PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

				if (playerHealth != null)
				{
					playerHealth.Damage(1); // Deal damage to the player
				}

				Destroy(gameObject); // Destroy the bullet
			}


		}



		#endregion

		//LifeTime
		#region LifeTime
		private void HandleLifetime()
		{
			_lifetimeTimer -= Time.deltaTime;

			if (_lifetimeTimer <= 0f)
			{
				Destroy(gameObject); // Destroy bullet after lifetime expires
			}
		}
		#endregion

	}
}


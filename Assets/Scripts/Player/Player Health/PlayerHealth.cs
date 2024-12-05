using UnityEngine;


namespace AstroAssault
{
	public class PlayerHealth : MonoBehaviour
	{

		//Serialize Fields
		#region Serialize Fields
		[Header("Player Hearts")]
		public int maxHearts = 6; // Maximum number of hearts
		public int currentHearts = 6; // Current number of hearts
		public GameObject heartContainerObject;
		#endregion

		//Private Variables
		#region Private Variables
		public bool IsDead { get; private set; } = false;

		private HeartContainer _heartContainer;
		public KeyCode invincibilityToggleKey = KeyCode.I; // Key to toggle invincibility
		private bool _isInvincible = false; // Whether the player is currently invincible
		private Vector3 _initialPosition;
		private PlayerController _playerController;

		#endregion

		//Initialization
		#region Initialization
		void Start()
		{
			_heartContainer = heartContainerObject.GetComponent<HeartContainer>();
			_heartContainer.SetMaxHearts(maxHearts);
			_playerController = FindObjectOfType<PlayerController>();
			_initialPosition = transform.position;

		}

		void Update()
		{
			// If the invincibilityToggleKey is pressed, toggle the _isInvincible variable
			if (Input.GetKeyDown(invincibilityToggleKey))
			{
				_isInvincible = !_isInvincible;
				Debug.Log("Invincibility toggled: " + _isInvincible);
			}
		}

		#endregion

		//Health System
		#region Health System
		public void SetPlayerHealth(int hearts)
		{
			currentHearts = hearts;
			_heartContainer.SetHearts(currentHearts);
		}

		public void Damage(int heartAmount)
		{
			// Check if the player is invincible
			if (_isInvincible)
			{
				Debug.Log("Player is invincible, no damage taken.");
				return; // Exit the method without applying damage
			}

			// Deduct the health based on heartAmount (1 heart = 2 health points)
			currentHearts -= heartAmount; // Multiply by 2 to reflect the hearts



			// Clamp the current health to ensure it does not go below 0
			currentHearts = Mathf.Clamp(currentHearts, 0, maxHearts * 2); // 10 max health

			// Update the UI
			_heartContainer.SetHearts(currentHearts);

			// If health reaches 0, trigger GameOver
			if (currentHearts <= 0 && !IsDead)
			{
				GameManager.gameManager.GameOver();
				IsDead = true; // Set IsDead to true
				GameManager.gameManager.GameOver();
			}
		}


		public void ResetPosition()
		{
			transform.position = _initialPosition; // Reset to the initial position
		}
		public void ResetDeathStatus() 
		{ 
			IsDead = false; 
		}

		#endregion
	}

}


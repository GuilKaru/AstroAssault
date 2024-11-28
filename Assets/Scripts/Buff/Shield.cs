using UnityEngine;

namespace AstroAssault
{
	public class Shield : MonoBehaviour
	{
		//Serialize Fields
		#region Serialize Fields
		[SerializeField]
		private int _maxHits = 3; // Number of hits the shield can take
		private int _currentHits;

		[Header("Animation")]
		[SerializeField]
		private Animator _animator;
		#endregion

		//References
		#region References
		private BuffManager _buffManager;

		//Animations
		private string _currentState;
		private string _idleAnim = "PlayerShield_Idle";
		#endregion

		//Initialization
		#region Initialization
		private void Start()
		{
			_currentHits = _maxHits;
			_buffManager = FindObjectOfType<BuffManager>();
			_animator = _animator = GetComponent<Animator>();
			ChangeAnimationState(_idleAnim);
		}
		#endregion

		//Collision
		#region Collision

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (collision.gameObject.CompareTag("EnemyBullet"))
			{
				// Absorb the hit
				_currentHits--;
				Destroy(collision.gameObject);

				// Check if the shield has reached its limit
				if (_currentHits <= 0)
				{
					DeactivateShield();
				}
			}

			if(collision.gameObject.CompareTag("Enemy"))
			{
				// Absorb the hit
				_currentHits--;
				Destroy(collision.gameObject);

				// Check if the shield has reached its limit
				if (_currentHits <= 0)
				{
					DeactivateShield();
					ResetShield();
				}
			}
		}
		#endregion

		//Buff References
		#region Buff References
		// Method to deactivate the shield
		private void DeactivateShield()
		{
			_buffManager.DeactivateShieldBuff(); // Notify BuffManager to handle visual effects, etc.
		}

		// Reset the shield when re-activated
		public void ResetShield()
		{
			_currentHits = _maxHits;
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
		#endregion
	}

}

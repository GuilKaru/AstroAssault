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
		#endregion

		//References
		#region References
		private BuffManager _buffManager;
		#endregion

		//Initialization
		#region Initialization
		private void Start()
		{
			_currentHits = _maxHits;
			_buffManager = FindObjectOfType<BuffManager>();
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
	}

}


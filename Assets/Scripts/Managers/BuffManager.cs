using UnityEngine;
using System.Collections;

namespace AstroAssault
{
	public class BuffManager : MonoBehaviour
	{
		//Buff Settings
		#region Buff Settings
		[Header("Shotgun Buff")]
		[SerializeField]
		private float _shotgunDuration = 5f;

		// Buff States
		private bool _isShotgunActive = false;
		private bool _isShieldActive = false;

		private Coroutine _shotgunCoroutine;
		#endregion

		//Player Reference
		#region Player Reference
		[Header("Player Reference")]
		[SerializeField]
		private PlayerController _playerController; // Reference to the PlayerController
		#endregion

		//Public Methods
		#region Public Methods

		public void ActivateShotgunBuff()
		{
			if (_isShotgunActive) return; // Prevent multiple activations

			_isShotgunActive = true;
			_playerController.ChangeShootingMode(2); // Switch to Shotgun mode

			if (_shotgunCoroutine != null) StopCoroutine(_shotgunCoroutine);
			_shotgunCoroutine = StartCoroutine(ShotgunBuffTimer());
		}

	
		public void ActivateShieldBuff()
		{
			if (_isShieldActive) return; // Prevent multiple activations
			_playerController.ActivateShield(true);
			Debug.Log("Shield Buff Activated!");
		}
		#endregion

		//Private Methods
		#region Private Methods

		private IEnumerator ShotgunBuffTimer()
		{
			yield return new WaitForSeconds(_shotgunDuration);

			_isShotgunActive = false;
			_playerController.ChangeShootingMode(1); // Switch back to Automatic mode
			Debug.Log("Shotgun Buff Deactivated!");
		}

		public void DeactivateShieldBuff()
		{
			_playerController.ActivateShield(false);
			Debug.Log("Shield Buff Deactivated!");
		}

		#endregion

	}

}

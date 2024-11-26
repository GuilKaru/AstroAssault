using UnityEngine;
namespace AstroAssault
{
	public class ShieldBuffPickUp : MonoBehaviour
	{

		private BuffManager _buffManager;

		private void Start()
		{
			_buffManager = FindObjectOfType<BuffManager>();  // Find the BuffManager in the scene
			if (_buffManager == null)
			{
				Debug.LogError("BuffManager not found in the scene!");
			}
		}
		private void OnTriggerEnter2D(Collider2D collider)
		{
			if (collider.gameObject.CompareTag("Player"))
			{
				BuffManager buffManager = collider.gameObject.GetComponent<BuffManager>();
				if (buffManager != null)
				{
					buffManager.ActivateShieldBuff(); // Activate the shield buff
				}

				Destroy(gameObject); // Destroy the pickup after activation
			}
		}
	}
}


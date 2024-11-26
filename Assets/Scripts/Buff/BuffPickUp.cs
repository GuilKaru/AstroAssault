
using UnityEngine;
namespace AstroAssault
{
	public class BuffPickup : MonoBehaviour
	{
		[SerializeField]
			private BuffManager _buffManager;

			[SerializeField]
			private string _buffType; // Example: "Shotgun" or "Shield"

			private void Awake()
			{
				_buffManager = FindAnyObjectByType<BuffManager>();

			}

			private void OnTriggerEnter2D(Collider2D collision)
			{
				if (collision.CompareTag("Player"))
				{
					if (_buffType == "Shotgun")
					{
						_buffManager.ActivateShotgunBuff();
					}
					else if (_buffType == "Shield")
					{
						_buffManager.ActivateShieldBuff();
					}

					Destroy(gameObject); // Remove the pickup after use
				}
			}
	}

}


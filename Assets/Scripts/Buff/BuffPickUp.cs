
using UnityEngine;
using UnityEngine.Rendering;
namespace AstroAssault
{
	public class BuffPickup : MonoBehaviour
	{
		[SerializeField]
		private BuffManager _buffManager;

		[SerializeField]
		private string _buffType; // Example: "Shotgun" or "Shield"

		[SerializeField]
		private float _speed = 20f;

		//Initialization
		#region Initialization
		private void Awake()
		{
			_buffManager = FindAnyObjectByType<BuffManager>();

		}

		private void Update()
		{
			Movement();
		}
		#endregion



		//Movement
		private void Movement()
		{
			transform.position += Vector3.left * _speed * Time.deltaTime;

			if (transform.position.x < -15f)  // Destroy when x is less than -10
			{
				Destroy(gameObject);
			}
		}


		//Collision
		#region Collision
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
		#endregion
	}

}


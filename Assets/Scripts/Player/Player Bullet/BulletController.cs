using UnityEngine;

namespace AstroAssault
{
	public class BulletController : MonoBehaviour
	{
		public Vector3 target; // Target position for the bullet

		private void Update()
		{
			if (!GameManager.gameManager.gameStarted) return;
			if (GameManager.gameManager.gamePaused) return;
			// Move towards the target
			transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * 10f);

			// Destroy the bullet if it reaches the target
			if (Vector3.Distance(transform.position, target) < 0.1f)
			{
				Destroy(gameObject);
			}
		}
	}

}


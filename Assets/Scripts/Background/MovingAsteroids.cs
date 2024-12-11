using UnityEngine;
namespace AstroAssault
{
	public class MovingAsteroids : MonoBehaviour
	{
		[Header("Waypoints Configuration")]
		[SerializeField] private Transform[] waypoints; // Array of waypoints
		[SerializeField] private float waypointSpeed = 5f; // Speed for waypoint movement
		[SerializeField] private bool loopWaypoints = true; // Should waypoints loop

		private int currentWaypointIndex = 0;

		[Header("Sprites")]
		[SerializeField] private Sprite[] asteroidSprites; // Array of asteroid sprites
		private SpriteRenderer spriteRenderer;

		private void Start()
		{
			// Cache the SpriteRenderer component
			spriteRenderer = GetComponent<SpriteRenderer>();

			if (spriteRenderer == null)
			{
				Debug.LogError("No SpriteRenderer found! Make sure this GameObject has a SpriteRenderer.");
			}
		}

		private void Update()
		{
			// Prevent movement if the game hasn't started
			if (!GameManager.gameManager.gameStarted) return;
			if (GameManager.gameManager.gamePaused) return;

			// Move along waypoints
			MoveAlongWaypoints();
		}

		private void MoveAlongWaypoints()
		{
			if (waypoints.Length == 0) return;

			// Move towards the current waypoint
			Transform targetWaypoint = waypoints[currentWaypointIndex];
			transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, waypointSpeed * Time.deltaTime);

			// Check if the object reached the current waypoint
			if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
			{
				// Change the sprite when reaching a waypoint
				ChangeSprite();

				// Increment the waypoint index
				currentWaypointIndex++;

				// Handle looping or teleporting back
				if (currentWaypointIndex >= waypoints.Length)
				{
					if (loopWaypoints)
					{
						// Teleport back to the first waypoint
						currentWaypointIndex = 0;
						transform.position = waypoints[currentWaypointIndex].position;
					}
					else
					{
						enabled = false; // Stop movement if looping is disabled
					}
				}
			}
		}

		private void ChangeSprite()
		{
			if (asteroidSprites.Length == 0 || spriteRenderer == null) return;

			// Choose a random sprite from the array
			int randomIndex = Random.Range(0, asteroidSprites.Length);
			spriteRenderer.sprite = asteroidSprites[randomIndex];
		}

		private void OnDrawGizmos()
		{
			// Visualize the waypoints in the editor
			if (waypoints != null)
			{
				Gizmos.color = Color.green;
				for (int i = 0; i < waypoints.Length; i++)
				{
					if (waypoints[i] != null)
						Gizmos.DrawSphere(waypoints[i].position, 0.2f);
				}
			}
		}
	}
}


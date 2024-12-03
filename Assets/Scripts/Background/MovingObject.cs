using UnityEngine;
using System.Collections;

namespace AstroAssault
{
	public class MovingObject : MonoBehaviour
	{
		[Header("Movement Mode")]
		[SerializeField] private bool useWaypoints = true; // Toggle between waypoints or circular movement

		[Header("Waypoints Configuration")]
		[SerializeField] private Transform[] waypoints; // Array of waypoints
		[SerializeField] private float waypointSpeed = 5f; // Speed for waypoint movement
		[SerializeField] private bool loopWaypoints = true; // Should waypoints loop

		private int currentWaypointIndex = 0;

		[Header("Circular Movement Configuration")]
		[SerializeField] private Transform centerPoint; // Center of the circle
		[SerializeField] private float radius = 5f; // Radius of the circle
		[SerializeField] private float angularSpeed = 2f; // Speed of rotation (radians per second)
		[SerializeField] private float initialAngleOffset = 0f; // Offset to start the object at a specific point on the circle

		private float currentAngle = 0f; // Current angle for circular motion

		private void Start()
		{
			if (!useWaypoints && centerPoint != null)
			{
				// Calculate the initial angle based on the current position and center point
				Vector3 offset = transform.position - centerPoint.position;
				currentAngle = Mathf.Atan2(offset.y, offset.x) + initialAngleOffset; // Add any custom offset
			}
		}

		private void Update()
		{
			// Prevent movement if the game hasn't started
			if (!GameManager.gameManager.gameStarted) return;

			if (useWaypoints)
			{
				MoveAlongWaypoints();
			}
			else
			{
				MoveInCircle();
			}
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
				// Increment the waypoint index
				currentWaypointIndex++;

				// Handle looping or stopping
				if (currentWaypointIndex >= waypoints.Length)
				{
					if (loopWaypoints)
					{
						// Teleport back to the first waypoint
						currentWaypointIndex = 0;
						transform.position = waypoints[currentWaypointIndex].position; // Teleport to the first waypoint
					}
					else
					{
						enabled = false; // Stop movement if looping is disabled
					}
				}
			}
		}

		private void MoveInCircle()
		{
			if (centerPoint == null) return;

			// Increment the angle based on the angular speed
			currentAngle += angularSpeed * Time.deltaTime;

			// Calculate the new position based on the angle
			float x = centerPoint.position.x + radius * Mathf.Cos(currentAngle);
			float y = centerPoint.position.y + radius * Mathf.Sin(currentAngle);

			// Update the object's position
			transform.position = new Vector3(x, y, transform.position.z);
		}

		private void OnDrawGizmos()
		{
			if (useWaypoints && waypoints != null)
			{
				// Draw waypoints
				Gizmos.color = Color.green;
				for (int i = 0; i < waypoints.Length; i++)
				{
					if (waypoints[i] != null)
						Gizmos.DrawSphere(waypoints[i].position, 0.2f);
				}
			}
			else if (centerPoint != null)
			{
				// Draw the circle
				Gizmos.color = Color.blue;
				Gizmos.DrawWireSphere(centerPoint.position, radius);
			}
		}
	}
}





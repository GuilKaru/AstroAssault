using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AstroAssault
{
	public class ScoreManager : MonoBehaviour
	{
		// Serialize Fields
		#region SerializeField
		[SerializeField]
		private TextMeshProUGUI _scoreText;
		#endregion

		// Variables
		#region Variables
		public int scoreCount;
		#endregion

		// Points Methods
		#region Point Methods
		public void AddScore(int points)
		{
			scoreCount = Mathf.Max(0, scoreCount + points);

			ScoreUpdate();

			Debug.Log("Score updated: " + scoreCount);

		}

		// Deduct score but keep it at a minimum of zero
		public void DeductScore(int points)
		{
			scoreCount = Mathf.Max(0, scoreCount + points); // Deduct points, but cap at zero
			ScoreUpdate();
			Debug.Log("Score deducted: " + scoreCount);
		}


		// Update the score display
		public void ScoreUpdate()
		{
			_scoreText.text = scoreCount.ToString();
		}


		public int GetScore()
		{
			return scoreCount;
		}

		#endregion


	}
}


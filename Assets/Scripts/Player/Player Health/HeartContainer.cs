using UnityEngine;
using UnityEngine.UI;

namespace AstroAssault
{

    public class HeartContainer : MonoBehaviour
    {
		//Serialize Fields
		#region Serialize Fields
		[Header("Hearts References")]
		[SerializeField]
		private Image heartPrefab;
		[SerializeField]
		private Transform heartsParent;
		[SerializeField]
		private Image[] hearts;
		#endregion

		//Setting Hearts
		#region Setting Hearts
		public void SetMaxHearts(int maxHearts)
		{
			for (int i = 0; i < hearts.Length; i++)
			{
				hearts[i].fillAmount = 1;
				hearts[i].gameObject.SetActive(i < maxHearts);
			}
		}

		public void SetHearts(int currentHealth)
		{

			Debug.Log($"This is the currentHealth ===> {currentHealth}");
			for (int i = 0; i < hearts.Length; i++)
			{


				if (i < currentHealth) // Full heart
				{
					hearts[i].fillAmount = 1f;
				}
				else // Empty heart
				{
					hearts[i].fillAmount = 0f;
				}

				//Caso de que cambien de opinion y se ponga los corazones a mitad
				/*
				if (i < currentHealth / 2) // Full heart
				{
					hearts[i].fillAmount = 1f;
				}
				else if (i == currentHealth / 2 && currentHealth % 2 != 0) // Half heart
				{
					hearts[i].fillAmount = 0.5f;
				}
				else // Empty heart
				{
					hearts[i].fillAmount = 0f;
				}*/
			}
		}

		#endregion


	}
}

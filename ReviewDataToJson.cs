using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EqualReality.AI;

namespace BrennanHatton.GPT
{
	
	public class ReviewDataToJson : MonoBehaviour
	{
		public string json;
		
		
		void Reset()
		{
			AIReviewScene reviewManager = this.GetComponent<AIReviewScene>();
			
			json = JsonUtility.ToJson(reviewManager.reviewData, true);
		}
	}

}
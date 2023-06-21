using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EqualReality.ReviewAI
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
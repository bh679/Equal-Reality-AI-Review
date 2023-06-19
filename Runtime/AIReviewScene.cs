using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EqualReality.AI
{
	[System.Serializable]
	public class ReviewQuestion
	{
		public string Question;
			
		public string WrapQuestion(string wrapper, string target = "#")
		{
			return wrapper.Replace(target,Question);
		}
	}
		
	[System.Serializable]
	public class ReviewData
	{
		public string intro;
		public ReviewQuestion[] reviewQuestions;
		public string conclusion;
		public int comments = 2;
			
		int currentQuestionId = 0;
		public void ToNextQuestion()
		{
			currentQuestionId = (currentQuestionId+ 1 ) % reviewQuestions.Length;
		}
			
		public string CurrentQuestion
		{
			get{
				return reviewQuestions[currentQuestionId].Question;
			}
		}
		public string NextQuestion
		{
			get{
					
				if(currentQuestionId >= reviewQuestions.Length-1)
					return conclusion;
						
				return reviewQuestions[currentQuestionId+1].Question;
					
			}
		}
			
		public bool AtLastQuestion()
		{
			return (currentQuestionId >= reviewQuestions.Length-1);
		}
	}
	
	public class AIReviewScene : MonoBehaviour
	{
		public string reviewDataText = "/AI/ReviewData.txt";
		public ReviewData reviewData;
		
		
	    // Start is called before the first frame update
	    void Start()
	    {	
		    StartCoroutine(TextFile.Load(reviewDataText, (result) => 
		    {
			    Debug.Log(result);
			    reviewData = JsonUtility.FromJson<ReviewData>(result); 
			    
			    AIReviewManager.Instance.StartReview(reviewData);
		    }));
	    }
	}

}

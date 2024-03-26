using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EqualReality.ReviewAI
{
		

	/// <summary>
	/// Question actions are Unity Events that can occcur for a particular point in question.
	/// eg: you want to enable bar graphs for when a voice line is read by the ai
	/// eg2: you want to disable bar graphs when the player has finished responded to a particuar question.
	/// </summary>
	[System.Serializable]
	public class QuestionActionsGroup
	{
		public UnityEvent action;
		
		public int QuestionID;
		
		public AIState state;
	}
	
	public class QuestionActions : MonoBehaviour
	{
		public AIReviewUI aIReviewUI;
		
		public List<QuestionActionsGroup> questionActions;
		
		void Reset()
		{
			aIReviewUI = GameObject.FindObjectOfType<AIReviewUI>();
		}
		
	    // Start is called before the first frame update
	    void Start()
		{
			aIReviewUI.OnMicOn.AddListener(ExecuteQuestionAction);
			aIReviewUI.OnMicRecording.AddListener(ExecuteQuestionAction);
			aIReviewUI.OnMicStopped.AddListener(ExecuteQuestionAction);
			aIReviewUI.OnSentToGPT.AddListener(ExecuteQuestionAction);
			aIReviewUI.OnResponseFromGPT.AddListener(ExecuteQuestionAction);
			aIReviewUI.OnVoicing.AddListener(ExecuteQuestionAction);
			aIReviewUI.OnVoiceDownloading.AddListener(ExecuteQuestionAction);
			aIReviewUI.OnVoicePlaying.AddListener(ExecuteQuestionAction);
			aIReviewUI.OnVoiceFinished.AddListener(ExecuteQuestionAction);
		}
	    
		
		//AIReviewManager.Instance.qid
		void ExecuteQuestionAction()
		{
			for(int i = 0; i < questionActions.Count; i++)
				if(questionActions[i].state == aIReviewUI.aiState && questionActions[i].QuestionID == AIReviewManager.Instance.qid)
					questionActions[i].action.Invoke();
		}
	    
		
		//AIReviewManager.Instance.qid
		void ExecuteQuestionAction(string str)
		{
			ExecuteQuestionAction();
		}
	}

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EqualReality.ReviewAI;

namespace EqualReality.ReviewAI.GPTAIIntergration
{
		
	public class LoudnessRefSetter : MonoBehaviour
	{
		public AIReviewUI aIReviewUI;
		
		public LoudnessToHeight[] loudnessToHeights;
		public LoudnessToImageFill[] loudnessToImageFills;
		public LoudnessEvent[] loudnessEvents;
		
		void Reset()
		{
			aIReviewUI = GameObject.FindObjectOfType<AIReviewUI>();
			loudnessToHeights = GameObject.FindObjectsOfType<LoudnessToHeight>(true);
			loudnessToImageFills = GameObject.FindObjectsOfType<LoudnessToImageFill>(true);
			loudnessEvents = GameObject.FindObjectsOfType<LoudnessEvent>(true);
		}
		
		
		void Start()
		{
			aIReviewUI.onSetAIRefts.AddListener(SetAIRefs);
		}
		
		void SetAIRefs()
		{
			for(int i = 0; i < loudnessToHeights.Length; i++)
			{
				loudnessToHeights[i].vr = aIReviewUI.voice;
			}
			
			for(int i = 0; i < loudnessToImageFills.Length; i++)
			{
				loudnessToImageFills[i].vr = aIReviewUI.voice;
			}
			
			for(int i = 0; i < loudnessEvents.Length; i++)
			{
				loudnessEvents[i].vr = aIReviewUI.voice;
			}
			
		}
	}

}
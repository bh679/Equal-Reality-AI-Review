using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace EqualReality.ReviewAI
{
		
	public class VRGuideConversationLine : MonoBehaviour
	{
		public AIReviewUI AIReviewManager;
		public TMP_Text text_content;
		public UnityEvent onSentToGPT, onResponse, onVoicing, onDownloadingVoice, onPlayingVoice;
		
		
		void Reset()
		{
			AIReviewManager = GameObject.FindAnyObjectByType<AIReviewUI>();
			text_content = GetComponentInChildren<TMP_Text>();
		}
		
	    // Start is called before the first frame update
	    void Start()
		{
			AIReviewManager.OnSentToGPT.AddListener(OnSentToGPT);
			AIReviewManager.OnResponseFromGPT.AddListener(OnResponseFromGPT);
			AIReviewManager.OnVoicing.AddListener(OnVoicing);
			AIReviewManager.OnVoiceDownloading.AddListener(OnVoiceDownloading);
			AIReviewManager.OnVoicePlaying.AddListener(OnPlayingVoice);
		}
		
		public void OnSentToGPT(string str)
		{
			AIReviewManager.OnSentToGPT.RemoveListener(OnSentToGPT);
			onSentToGPT.Invoke();
		}
	    
		public void OnResponseFromGPT(string text)
		{
			AIReviewManager.OnResponseFromGPT.RemoveListener(OnResponseFromGPT);
			onResponse.Invoke();
			
			text_content.text = text;
		}
		
		public void OnVoicing()
		{
			AIReviewManager.OnVoicing.RemoveListener(OnVoicing);
			onVoicing.Invoke();
		}
		
		public void OnVoiceDownloading()
		{
			AIReviewManager.OnVoiceDownloading.RemoveListener(OnVoiceDownloading);
			onDownloadingVoice.Invoke();
		}
		
		public void OnPlayingVoice()
		{
			AIReviewManager.OnVoicePlaying.RemoveListener(OnPlayingVoice);
			onPlayingVoice.Invoke();
		}
	}
}
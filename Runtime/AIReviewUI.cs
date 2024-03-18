using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using EqualReality.ReviewAI.GPTAIIntergration;
using TMPro;

namespace EqualReality.ReviewAI
{
	public enum AIState
	{
		MicOn,
		MicRecording,
		MicStopped,
		SentToGPT,
		ReponseFromGPT,
		Voicing,
		DownloadingVoice,
		PlayingVoice,
		None
	}
	
	

	/// <summary>
	/// Question actions are Unity Events that can occcur for a particular point in question.
	/// eg: you want to enable bar graphs for when a voice line is read by the ai
	/// eg2: you want to disable bar graphs when the player has finished responded to a particuar question.
	/// </summary>
	[System.Serializable]
	public class QuestionActions
	{
		public UnityEvent action;
		
		public int QuestionID;
		
		public AIState state;
	}
	
	public class AIReviewUI : MonoBehaviour
	{
		OpenAIDemo gpt;
		VoiceRecorder voice;
		ELSpeaker elSpeaker;
		public Transform MicOn, MicRecording, MicStopped, SentToGPT, Response, Voicing, DownloadingVoice, PlayingVoice;
		
		public AIState aiState = AIState.None;
			
		public UnityEvent  OnMicOn, OnMicRecording, OnMicStopped;
		public StringEvent OnSentToGPT, OnResponseFromGPT;
		public UnityEvent OnVoicing, OnVoiceDownloading, OnVoicePlaying, OnVoiceFinished;
		
		public LoudnessToHeight[] loudnessToHeights;
		public LoudnessToImageFill[] loudnessToImageFills;
		public RecordingDeviceToText recordingDeviceCheck;
		public TMP_InputField voice_Input, aiResponse;
		
		
		public List<QuestionActions> questionActions;
	
		void Reset()
		{
			MicOn = transform. Find( "Mic On" );
			MicRecording = transform. Find( "Mic Recording");
			MicStopped = transform. Find( "Mic Stopped");
			SentToGPT = transform. Find ("Sent to GPT");
			Response = transform. Find( "Response");
			Voicing = transform. Find( "Voicing");
			DownloadingVoice = transform. Find( "Downloading Voice");
			PlayingVoice = transform. Find( "Playing Voice");
			
			loudnessToHeights = this.GetComponentsInChildren<LoudnessToHeight>();
			loudnessToImageFills = this.GetComponentsInChildren<LoudnessToImageFill>();
			
		}
		

		public void SetAIRefs(OpenAIDemo _gpt, VoiceRecorder _voice, ELSpeaker _elSpeaker)
		{
			this.gpt = _gpt;
			this.voice = _voice;
			this.elSpeaker = _elSpeaker;
			this.voice.input = voice_Input;
			
			for(int i = 0; i < loudnessToHeights.Length; i++)
			{
				loudnessToHeights[i].vr = voice;
			}
			
			for(int i = 0; i < loudnessToImageFills.Length; i++)
			{
				loudnessToImageFills[i].vr = voice;
			}
			
			
			recordingDeviceCheck.vr = voice;
			
			voice.onMicMonitor.AddListener(()=>{
				SetAllOff();
				MicOn.gameObject.SetActive(true);
				aiState = AIState.MicOn;
				OnMicOn.Invoke();
				ExecuteQuestionAction();
			});
			
			voice.onMicRecording.AddListener(()=>{
				SetAllOff();
				MicRecording.gameObject.SetActive(true);
				aiState = AIState.MicRecording;
				OnMicRecording.Invoke();
				ExecuteQuestionAction();
			});
			
			voice.onMicStopped.AddListener(()=>{
				SetAllOff();
				MicStopped.gameObject.SetActive(true);
				aiState = AIState.MicStopped;
				OnMicStopped.Invoke();
				ExecuteQuestionAction();
			});
			
			gpt.onSendGPT.AddListener((string str)=>{
				SetAllOff();
				SentToGPT.gameObject.SetActive(true);
				aiState = AIState.SentToGPT;
				OnSentToGPT.Invoke(str);
				ExecuteQuestionAction();
			});
			
			gpt.onRecieveResponse.AddListener((string str)=>{
				SetAllOff();
				Response.gameObject.SetActive(true);
				aiState = AIState.ReponseFromGPT;
				OnResponseFromGPT.Invoke(str);
				if(aiResponse != null) aiResponse.text = str;
				ExecuteQuestionAction();
			});
			
			elSpeaker.onSendForVoice.AddListener(()=>{
				SetAllOff();
				Voicing.gameObject.SetActive(true);
				aiState = AIState.Voicing;
				OnVoicing.Invoke();
				ExecuteQuestionAction();
			});
			
			elSpeaker.onDownloadingVoice.AddListener(()=>{
				SetAllOff();
				DownloadingVoice.gameObject.SetActive(true);
				aiState = AIState.DownloadingVoice;
				OnVoiceDownloading.Invoke();
				ExecuteQuestionAction();
			});
			
			elSpeaker.onPlayingVoice.AddListener(()=>{
				SetAllOff();
				PlayingVoice.gameObject.SetActive(true);
				aiState = AIState.PlayingVoice;
				OnVoicePlaying.Invoke();
				ExecuteQuestionAction();
			});
			
			elSpeaker.onFinishPlayingVoice.AddListener(()=>{
				PlayingVoice.gameObject.SetActive(false);
				aiState = AIState.None;
				OnVoiceFinished.Invoke();
				ExecuteQuestionAction();
			});
		}
		
		void SetAllOff()
		{
			MicOn.gameObject.SetActive(false);
			MicRecording.gameObject.SetActive(false);
			MicStopped.gameObject.SetActive(false);
			SentToGPT.gameObject.SetActive(false);
			Response.gameObject.SetActive(false);
			Voicing.gameObject.SetActive(false);
			DownloadingVoice.gameObject.SetActive(false);
			PlayingVoice.gameObject.SetActive(false);
		}
		
		void Start()
		{
			AIReviewManager.Instance.onAIReset.AddListener(SetAIRefs);
		}
		
		//AIReviewManager.Instance.qid
		void ExecuteQuestionAction()
		{
			for(int i = 0; i < questionActions.Count; i++)
				if(questionActions[i].state == aiState && questionActions[i].QuestionID == AIReviewManager.Instance.qid)
					questionActions[i].action.Invoke();
		}
	}
}

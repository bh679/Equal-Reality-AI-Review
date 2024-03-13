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
		public TMP_InputField voice_Input;
	
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
			});
			
			voice.onMicRecording.AddListener(()=>{
				SetAllOff();
				MicRecording.gameObject.SetActive(true);
				aiState = AIState.MicRecording;
				OnMicRecording.Invoke();
			});
			
			voice.onMicStopped.AddListener(()=>{
				SetAllOff();
				MicStopped.gameObject.SetActive(true);
				aiState = AIState.MicStopped;
				OnMicStopped.Invoke();
			});
			
			gpt.onSendGPT.AddListener((string str)=>{
				SetAllOff();
				SentToGPT.gameObject.SetActive(true);
				aiState = AIState.SentToGPT;
				OnSentToGPT.Invoke(str);
			});
			
			gpt.onRecieveResponse.AddListener((string str)=>{
				SetAllOff();
				Response.gameObject.SetActive(true);
				aiState = AIState.ReponseFromGPT;
				OnResponseFromGPT.Invoke(str);
			});
			
			elSpeaker.onSendForVoice.AddListener(()=>{
				SetAllOff();
				Voicing.gameObject.SetActive(true);
				aiState = AIState.Voicing;
				OnVoicing.Invoke();
			});
			
			elSpeaker.onDownloadingVoice.AddListener(()=>{
				SetAllOff();
				DownloadingVoice.gameObject.SetActive(true);
				aiState = AIState.DownloadingVoice;
				OnVoiceDownloading.Invoke();
			});
			
			elSpeaker.onPlayingVoice.AddListener(()=>{
				SetAllOff();
				PlayingVoice.gameObject.SetActive(true);
				aiState = AIState.PlayingVoice;
				OnVoicePlaying.Invoke();
			});
			
			elSpeaker.onFinishPlayingVoice.AddListener(()=>{
				PlayingVoice.gameObject.SetActive(false);
				aiState = AIState.None;
				OnVoiceFinished.Invoke();
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
	}
}

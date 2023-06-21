using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI.DemoScript;
using OpenAI.Integrations.ElevenLabs;
using OpenAI.Integrations.VoiceRecorder;
using TMPro;

namespace EqualReality.ReviewAI
{
	public class AIReviewUI : MonoBehaviour
	{
		OpenAIDemo gpt;
		VoiceRecorder voice;
		ELSpeaker elSpeaker;
		public Transform MicOn, MicRecording, MicStopped, SentToGPT, Response, Voicing, DownloadingVoice, PlayingVoice;
		
		public LoudnessToHeight[] loudnessToHeights;
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
			
			recordingDeviceCheck.vr = voice;
			
			voice.onMicMonitor.AddListener(()=>{
				SetAllOff();
				MicOn.gameObject.SetActive(true);
			});
			
			voice.onMicRecording.AddListener(()=>{
				SetAllOff();
				MicRecording.gameObject.SetActive(true);
			});
			
			voice.onMicStopped.AddListener(()=>{
				SetAllOff();
				MicStopped.gameObject.SetActive(true);
			});
			
			gpt.onSendGPT.AddListener((string str)=>{
				SetAllOff();
				SentToGPT.gameObject.SetActive(true);
			});
			
			gpt.onRecieveResponse.AddListener((string str)=>{
				SetAllOff();
				Response.gameObject.SetActive(true);
			});
			
			elSpeaker.onSendForVoice.AddListener(()=>{
				SetAllOff();
				Voicing.gameObject.SetActive(true);
			});
			
			elSpeaker.onDownloadingVoice.AddListener(()=>{
				SetAllOff();
				DownloadingVoice.gameObject.SetActive(true);
			});
			
			elSpeaker.onPlayingVoice.AddListener(()=>{
				SetAllOff();
				PlayingVoice.gameObject.SetActive(true);
			});
			
			elSpeaker.onFinishPlayingVoice.AddListener(()=>{
				PlayingVoice.gameObject.SetActive(false);
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

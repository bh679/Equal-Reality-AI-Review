using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI.DemoScript;
using OpenAI.Integrations.ElevenLabs;
using OpenAI.Integrations.VoiceRecorder;

namespace EqualReality.ReviewAI
{
	public class AiAnimationController : MonoBehaviour
	{
		public Animator VRGuideAnimator;
		SpeechBlend speechBlend;
		OpenAIDemo gpt;
		VoiceRecorder voice;
		ELSpeaker elSpeaker;
		
		void Reset()
		{
			VRGuideAnimator = GameObject.FindFirstObjectByType<Animator>();
		}
		
		public void SetAIRefs(OpenAIDemo _gpt, VoiceRecorder _voice, ELSpeaker _elSpeaker)
		{
			this.gpt = _gpt;
			this.voice = _voice;
			this.elSpeaker = _elSpeaker;
			
			speechBlend.enabled = false;
			
			speechBlend.voiceAudioSource = elSpeaker.GetComponent<AudioSource>();
			
			elSpeaker.onPlayingVoice.AddListener(()=>{
				speechBlend.enabled = true;
				speechBlend.lipsyncActive = true;
				VRGuideAnimator.SetTrigger("presenting");
			
			});
			
			elSpeaker.onFinishPlayingVoice.AddListener(()=>{
			
				speechBlend.lipsyncActive = false;
				VRGuideAnimator.SetTrigger("idle_stand");
			
			});
		}
		
		void Start()
		{
			AIReviewManager.Instance.onAIReset.AddListener(SetAIRefs);
			speechBlend = VRGuideAnimator.GetComponent<SpeechBlend>();
		}
	}
}
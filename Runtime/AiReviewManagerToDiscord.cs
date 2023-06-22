using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI.DemoScript;
using OpenAI.Integrations.ElevenLabs;
using OpenAI.Integrations.VoiceRecorder;
using BrennanHatton.Discord;

namespace EqualReality.ReviewAI
{
		
	public class AiReviewManagerToDiscord : MonoBehaviour
	{
		public AIReviewManager reviewAi;
		public DiscordLogManager discord;
		
		void Reset()
		{
			reviewAi = GameObject.FindObjectOfType<AIReviewManager>();
			discord = GameObject.FindObjectOfType<DiscordLogManager>();
		}
		
		void SetListerners(OpenAIDemo_Ext openAi, VoiceRecorder_Ext _voice, ELSpeaker _elSpeaker)
		{
			openAi.onRecieveResponse.AddListener(discord.SendWebhook);
			openAi.onSendGPT.AddListener(discord.SendWebhook);
		}
		
		// Start is called before the first frame update
		void Start()
		{
			reviewAi.onAIReset.AddListener(SetListerners);
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using OpenAI.DemoScript;
using OpenAI.Integrations.ElevenLabs;
using OpenAI.Integrations.VoiceRecorder;
using BrennanHatton.GPT;

namespace EqualReality.ReviewAI
{
	

	/// <summary>
	/// A UnityEvent with a Grabbable as the parameter
	/// </summary>
	[System.Serializable]
	public class OpenAIDemoEvent : UnityEvent<OpenAIDemo> { }
	
	/// <summary>
	/// A UnityEvent with a Grabbable as the parameter
	/// </summary>
	[System.Serializable]
	public class AIResetEvent : UnityEvent<OpenAIDemo, VoiceRecorder, ELSpeaker> { }

	public class AIReviewManager : MonoBehaviour
	{
		
		public OpenAIDemo aiPrefab;
		OpenAIDemo gpt;
		VoiceRecorder voice;
		ELSpeaker elSpeaker;
		ReviewData reviewData;
		
		public string[] guidePromptText;
		string[] guidePrompts;
		public string preQuestionText, dataFormatExmpleText, genericCallToActionText, reviewDataText, acknowledgeCTAText, finalCTAText;
		string preQuestion, dataFormatExmple, genericCallToAction, acknowledgeCTA, finalCTA;
		int comment = 0, qid = 0;
		public UnityEvent onComplete, questionDone;
		public AIResetEvent onAIReset;
		
		
		public List<InteractionData> interactions = new List<InteractionData>();
		InteractionData currentInteraction;
		
		public static AIReviewManager Instance { get; private set; }
		private void Awake() 
		{ 
			// If there is an instance, and it's not me, delete myself.
    
			if (Instance != null && Instance != this) 
			{ 
				Destroy(this); 
			} 
			else 
			{ 
				Instance = this; 
			} 
		}
		
		void Reset()
		{
			gpt = GameObject.FindObjectOfType<OpenAIDemo>();
			voice = GameObject.FindObjectOfType<VoiceRecorder>();
		}
		
		int loaded = 0;
		// Start is called before the first frame update
		void Start()
		{
			
			guidePrompts = new string[guidePromptText.Length];
			for(int i = 0 ;i < guidePromptText.Length; i++)
			{
				StartCoroutine(LoadTextFile(guidePromptText[i], (result, i) => 
				{
					Debug.Log(result);
					guidePrompts[i] = result; 
					loaded++;
				}, i));
			}
			
			StartCoroutine(TextFile.Load(preQuestionText, (result) => 
			{
				Debug.Log(result);
				preQuestion = result; 
			}));
			
			StartCoroutine(TextFile.Load(dataFormatExmpleText, (result) => 
			{
				Debug.Log(result);
				dataFormatExmple = result; 
				loaded++;
			}));
			
			StartCoroutine(TextFile.Load(genericCallToActionText, (result) => 
			{
				Debug.Log(result);
				genericCallToAction = result; 
				loaded++;
			}));
			
			StartCoroutine(TextFile.Load(acknowledgeCTAText, (result) => 
			{
				Debug.Log(result);
				acknowledgeCTA = result; 
				loaded++;
			}));
			
			StartCoroutine(TextFile.Load(finalCTAText, (result) => 
			{
				Debug.Log(result);
				finalCTA = result; 
				loaded++;
			}));
		}
		
		void SetGPT()
		{
			if(gpt != null)
				DestroyImmediate(gpt.gameObject);
				
			gpt = Instantiate(aiPrefab,this.transform);
			gpt.transform.localPosition = Vector3.zero;
			
			gpt.prompWrapper.prePrompt2 = new string[3 + guidePromptText.Length];
			for(int i = 0 ;i < guidePromptText.Length; i++)
			{
				gpt.prompWrapper.prePrompt2[i] = guidePrompts[i];
			}
			
			gpt.prompWrapper.prePrompt2[guidePromptText.Length + 1] = dataFormatExmple;
			gpt.prompWrapper.prePrompt2[guidePromptText.Length + 2] = genericCallToAction;
			
			gpt.onSendGPT.AddListener(SaveSentData);
			gpt.onRecieveResponse.AddListener(SaveReceivedData);
			
			voice = gpt.GetComponent<VoiceRecorder>();
			
			
			elSpeaker = gpt.GetComponent<ELSpeaker>();
			elSpeaker.onFinishPlayingVoice.AddListener(NextInteraction);
			
			onAIReset.Invoke(gpt, voice, elSpeaker);
		}
		
		public void SaveSentData(string sentData)
		{
			RequestData requestData = new RequestData();
			requestData.prompt = sentData;
			requestData.max_tokens = gpt.max_tokens;
			requestData.frequency_penalty = (int)gpt.frequency_penalty;
			requestData.presence_penalty = (int)gpt.presence_penalty;
			requestData.temperature = (int)gpt.temperature;
			requestData.top_p = (int)gpt.top_p;

			currentInteraction = new InteractionData(requestData);
		}
		
		public void SaveReceivedData(string receivedData)
		{
			currentInteraction.SetResponse(receivedData);
			interactions.Add(currentInteraction);
			
		}
		
		public void StartReview(ReviewData _reviewData)
		{
			reviewData = _reviewData;
			
			SetGPT();
			finished = false;
			timeForConclusion = false;
			
			elSpeaker.SpeakSentence(reviewData.intro + reviewData.reviewQuestions[0].Question);
		}
		
		bool finished = false, timeForConclusion;
		void NextInteraction()
		{
			if(finished)
			{
				onComplete.Invoke();
				return;
			}
			
			SetGPT();
			
			
			Debug.Log(comment + " >= " + reviewData.comments);
			
			if(timeForConclusion)
			{
				elSpeaker.SpeakSentence(reviewData.conclusion);
				finished = true;
			}
			else if(comment >= reviewData.comments)
			{
				
				Debug.Log(reviewData.AtLastQuestion());
				if(reviewData.AtLastQuestion())
				{
					gpt.prompWrapper.prePrompt2[guidePromptText.Length + 2] = finalCTA;//.Replace("#",reviewData.conclusion);
					timeForConclusion = true;
					
				}
				else
					gpt.prompWrapper.prePrompt2[guidePromptText.Length + 2] = acknowledgeCTA.Replace("#",reviewData.NextQuestion);
				
				
				AskQuestion(reviewData.CurrentQuestion);
				reviewData.ToNextQuestion();
				comment = 0;
			}
			else
			{
				gpt.prompWrapper.prePrompt2[guidePromptText.Length + 2] = genericCallToAction;
				NextComment();
				return;
			}
			
		}
		
		void AskQuestion(string question)
		{
			Debug.Log("Asking Question");
			
			ReviewQuestion nextQuestion = new ReviewQuestion();
			nextQuestion.Question = question;
			
			gpt.prompWrapper.prePrompt2[guidePromptText.Length] = 
				nextQuestion.WrapQuestion(preQuestion);
			
			voice.ResetListerning();
			voice.MonitorMic();
		}
		
		void NextComment()
		{
			//SetGPT();
			
			
			Debug.Log("NextComment");
			
			comment++;
			ReviewQuestion nextQuestion = new ReviewQuestion();
			
			if(interactions.Count == 0)
				nextQuestion.Question = reviewData.CurrentQuestion;
				else
			nextQuestion.Question = InteractionData.MostRecentInteraction(interactions).generatedText;//InteractionData.MostRecentInteraction(gpt.interactions).generatedText;
			
			
			gpt.prompWrapper.prePrompt2[guidePromptText.Length] = 
				nextQuestion.WrapQuestion(preQuestion);
			
			voice.ResetListerning();
			voice.MonitorMic();
		}
		
		public IEnumerator LoadTextFile(string filPath, System.Action<string, int> result, int i)
		{
			string path = "file://" + Application.streamingAssetsPath + filPath;
			Debug.Log(path);
			using (UnityWebRequest www = UnityWebRequest.Get(path))
			{
				yield return www.SendWebRequest();
				if (www.result != UnityWebRequest.Result.Success)
				{
					Debug.Log(www.error);
					result(null, i); // You can call the action with null or with some error message.
				}
				else
				{
					//Debug.Log(www.downloadHandler.text);
					result(www.downloadHandler.text, i);
				}
			}
		}
		
		

	}

}
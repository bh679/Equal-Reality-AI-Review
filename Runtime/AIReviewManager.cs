using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using EqualReality.ReviewAI.GPTAIIntergration;
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
			Debug.Log("Start");
			guidePrompts = new string[guidePromptText.Length];
			Debug.Log("Load prompts");
			for(int i = 0 ;i < guidePromptText.Length; i++)
			{
				Debug.Log("Prompt:"+i);
				StartCoroutine(LoadTextFile(guidePromptText[i], (result, i) => 
				{
					Debug.Log(result);
					guidePrompts[i] = result; 
					Debug.Log("guidePrompt Loaded: "+ result);
					loaded++;
				}, i));
			}
			
			Debug.Log("preQuestion prompts");
			Debug.Log("preQuestionText: "+ preQuestionText);
			StartCoroutine(TextFile.Load(preQuestionText, (result) => 
			{
				Debug.Log(result);
				preQuestion = result; 
				Debug.Log("preQuestion Loaded: "+ result);
			}));
			
			Debug.Log("dataFormatExmple prompts");
			Debug.Log("dataFormatExmpleText: "+ dataFormatExmpleText);
			StartCoroutine(TextFile.Load(dataFormatExmpleText, (result) => 
			{
				Debug.Log(result);
				dataFormatExmple = result; 
				Debug.Log("dataFormatExmple Loaded: "+ result);
				loaded++;
			}));
			
			Debug.Log("genericCallToAction prompts");
			Debug.Log("genericCallToActionText: "+ genericCallToActionText);
			StartCoroutine(TextFile.Load(genericCallToActionText, (result) => 
			{
				Debug.Log(result);
				genericCallToAction = result; 
				Debug.Log("genericCallToAction Loaded: "+ result);
				loaded++;
			}));
			
			Debug.Log("acknowledgeCTA prompts");
			Debug.Log("acknowledgeCTAText: "+ acknowledgeCTAText);
			StartCoroutine(TextFile.Load(acknowledgeCTAText, (result) => 
			{
				Debug.Log(result);
				acknowledgeCTA = result; 
				Debug.Log("acknowledgeCTA Loaded: "+ result);
				loaded++;
			}));
			
			Debug.Log("finalCTA prompts");
			Debug.Log("finalCTAText: "+ finalCTAText);
			StartCoroutine(TextFile.Load(finalCTAText, (result) => 
			{
				Debug.Log(result);
				finalCTA = result; 
				Debug.Log("finalCTA Loaded: "+ result);
				loaded++;
			}));
		}
		
		void SetGPT()
		{
			Debug.Log("SetGPT");
			if(gpt != null)
				DestroyImmediate(gpt.gameObject);
				
			gpt = Instantiate(aiPrefab,this.transform);
			gpt.transform.localPosition = Vector3.zero;
			
			Debug.Log("gpt:" + gpt);
			gpt.prompWrapper.prePrompt2 = new string[3 + guidePromptText.Length];
			for(int i = 0 ;i < guidePromptText.Length; i++)
			{
				gpt.prompWrapper.prePrompt2[i] = guidePrompts[i];
			}
			
			gpt.prompWrapper.prePrompt2[guidePromptText.Length + 1] = dataFormatExmple;
			gpt.prompWrapper.prePrompt2[guidePromptText.Length + 2] = genericCallToAction;
			
			Debug.Log("gpt prompts wrapped");
			gpt.onSendGPT.AddListener(SaveSentData);
			gpt.onRecieveResponse.AddListener(SaveReceivedData);
			Debug.Log("gpt listerners set");
			
			voice = gpt.GetComponent<VoiceRecorder>();
			
			Debug.Log("voice:"+voice);
			
			elSpeaker = gpt.GetComponent<ELSpeaker>();
			Debug.Log("elSpeaker:"+elSpeaker);
			elSpeaker.onFinishPlayingVoice.AddListener(NextInteraction);
			Debug.Log("elSpeaker listener");
			
			onAIReset.Invoke(gpt, voice, elSpeaker);
			Debug.Log("AI Reset finished");
		}
		
		public void SaveSentData(string sentData)
		{
			Debug.Log("SaveSentData");
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
			Debug.Log("SaveReceivedData");
			currentInteraction.SetResponse(receivedData);
			interactions.Add(currentInteraction);
			
		}
		
		public void StartReview(ReviewData _reviewData)
		{
			Debug.Log("StartReview");
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
		
		public IEnumerator LoadTextFile(string filePath, System.Action<string, int> result, int i)
		{
			string path;
			
#if UNITY_ANDROID && !UNITY_EDITOR
			// For Android, directly use the path without "file://" prefix
			path = Application.streamingAssetsPath + filePath;
#else
			// For other platforms, prepend "file://" to the path
			path = "file://" + Application.streamingAssetsPath + filePath;
#endif

			Debug.Log("path in LoadTextFile: "+path);
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
					result(www.downloadHandler.text, i);
				}
			}
		}

		
		

	}

}
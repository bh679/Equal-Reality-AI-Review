using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using OpenAi.Api.V1;
using OpenAi.Unity.V1;
using System.IO;
using System.Linq;
using TMPro;
using BrennanHatton.GPT;
using UnityEngine.Events;
using OpenAI.DemoScript;

namespace EqualReality.ReviewAI.GPTAIIntergration
{

	/// <summary>
	/// A UnityEvent with a string as the parameter
	/// </summary>
	[System.Serializable]
	public class StringEvent : UnityEvent<string> { }
	
	public class OpenAIDemo : MonoBehaviour
	{
		public List<ChatMessageV1> messages = new List<ChatMessageV1>(); // Used as the chat history for context. If it exceeds the amount of tokens allowed, you'll get an error. * Need to add logic to trim oldest tokens.
        
		[SerializeField]
		public OpenAiCompleterV1 completer;
		[SerializeField]
		public string model = "gpt-3.5-turbo";
		[SerializeField]
		public TMP_Text prompt;
		[SerializeField]
		public TMP_Text response;
        
		public List<InteractionData> interactions = new List<InteractionData>();

		[Tooltip("This is how you set the NPC's personality and behavior")]
		public PromptWrapper prompWrapper;// = "Act as a pirate and speak in a pirate dialect.";

		[
		Range(1, 32768),
		Tooltip(
		"The maximum number of tokens to generate. Requests can use up to 4000 tokens shared between prompt and completion. (One token is roughly 4 characters for normal English text)"
		)
		]
		public int max_tokens = 1024;
		[
		Range(0.0f, 1.0f),
		Tooltip(
		"Controls randomness: Lowering results in less random completions. As the temperature approaches zero, the model will become deterministic and repetitive."
		)
		]
		public float temperature = 0.7f;
		[
		Range(0.0f, 1.0f),
		Tooltip(
		"Controls diversity via nucleus sampling: 0.5 means half of all likelihood-weighted options are considered."
		)
		]
		public float top_p = 1;
		[Tooltip(
		"Where the API will stop generating further tokens. The returned text will not contain the stop sequence."
		)]
		public string stop;
		[
		Range(0.0f, 2.0f),
		Tooltip(
		"How much to penalize new tokens based on their existing frequency in the text so far. Decreases the model's likelihood to repeat the same line verbatim."
		)
		]
		public float frequency_penalty = 0;
		[
		Range(0.0f, 2.0f),
		Tooltip(
		"How much to penalize new tokens based on whether they appear in the text so far. Increases the model's likelihood to talk about new topics."
		)
		]
		public float presence_penalty = 0;
        
		public StringEvent onSendGPT, onRecieveResponse, onFailed;

		public async Task SendOpenAIRequest(string promptText)
		{
	    	
			string wrappedPrompt = prompWrapper.wrapPromopt(promptText);
			ApiResult<CompletionV1> comp = null;
			ApiResult<ChatCompletionV1> chatComp = null;
			Debug.Log("Sending text to OpenAI for a response... " + promptText);
			Debug.Log(wrappedPrompt);
			if (model.Contains("gpt"))
			{
				RequestData requestdata = new RequestData();
				requestdata.model = model;
				requestdata.prompt = wrappedPrompt;
				requestdata.max_tokens = max_tokens;
				requestdata.frequency_penalty = (int)frequency_penalty;
				requestdata.temperature = temperature;
				requestdata.top_p = (int)top_p;
	            
				onSendGPT.Invoke(wrappedPrompt);
				chatComp = await SendChatGPTRequest(wrappedPrompt);
				Debug.Log("OpenAI Request complete, checking for success...");
				if (chatComp.IsSuccess)
				{
            
					onRecieveResponse.Invoke(chatComp.Result.choices[0].message.content);
	                
					InteractionData newInteraction = new InteractionData(requestdata);
	    	
					interactions.Add(newInteraction);
					newInteraction.SetResponse(chatComp.Result.choices[0].message.content);
	                
					if(response != null)
					{
						response.text = chatComp.Result.choices[0].message.content
							.Replace("\\n", "")
							.Replace("\\\"", "\"")
							.Replace("\\t", "\t")
							.Replace("\"", "");
					}
				}
				else
				{
					string responseStatus = $"ERROR: StatusCode: {chatComp.HttpResponse.responseCode} - {chatComp.HttpResponse.error}";
					if(response != null)
						response.text = responseStatus;
					onFailed.Invoke(responseStatus);
                	
					SendOpenAIRequest(promptText);//this calls onSendtwice, and can enter an infinite loop. Fix this later.
				}
			}
			else
			{
				//Use this method when calling a model other than gpt
				comp = await completer._gateway.Api.Engines
					.Engine(model)
					.Completions.CreateCompletionAsync(
					new CompletionRequestV1()
					{
						prompt = wrappedPrompt,
						max_tokens = completer.Args.max_tokens,
						temperature = completer.Args.temperature,
						top_p = completer.Args.top_p,
						stop = completer.Args.stop,
						frequency_penalty = completer.Args.frequency_penalty,
						presence_penalty = completer.Args.presence_penalty
                        }
					);

				if (comp.IsSuccess)
				{
					response.text = comp.Result.choices[0].text
						.Replace("\\n", "")
						.Replace("\\\"", "\"")
						.Replace("\\t", "\t");
				}
				else
				{
					response.text =
						$"ERROR: StatusCode: {comp.HttpResponse.responseCode} - {comp.HttpResponse.error}";
				}
				;
			}
            
		}
        
		// Use this method when calling a GPT model. * Needs logic to count how many tokens the current array is, and prune any old messages so that it doesn't exceed the models max token limit.
		public async Task<ApiResult<ChatCompletionV1>> SendChatGPTRequest(string message)
		{
	    	
	    	
			SOAuthArgsV1 auth = completer.Auth;
			OpenAiApiV1 api = new OpenAiApiV1(auth.ResolveAuth());
			Debug.Log("Sending GPT request...");

			// If messages list is empty, add the system instructions message
			if (messages.Count == 0)
			{
				messages.Add(new ChatMessageV1() { role = "system", content = "" });
			}
			// Add the user's message to the existing messages list
			messages.Add(new ChatMessageV1() { role = "user", content = message });
			ApiResult<ChatCompletionV1> chatComp =
				await api.ChatCompletions.CreateChatCompletionAsync(
				new ChatCompletionRequestV1()
				{
					model = "gpt-3.5-turbo",
					messages = messages.ToArray() // Pass the updated messages list to the API request
                    }
				);
                
			Debug.Log(chatComp);
			Debug.Log(chatComp.Result);
		    
			if(chatComp.Result == null)
			{
				chatComp.IsSuccess = false;
				return chatComp;
			}
		
		    
			// Add the assistant's response to the messages list
			messages.Add(new ChatMessageV1() { role = "assistant", content = chatComp.Result.choices[0].message.content });
            
			return chatComp;
		}
        
        
            
		/*List<string> failedCalls = new List<string>();
		void Update()
		{
		if(failedCalls.Count > 0)
		{
		StartCoroutine(_SpeakSentence(failedCalls[0]));
		failedCalls.RemoveAt(0);
		}
		}*/
            
	}
}

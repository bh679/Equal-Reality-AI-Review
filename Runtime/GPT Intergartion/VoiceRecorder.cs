// This script listens to the microphone and when the input crosses the set threshold, it begins recording. If the volume of the input falls below that threshold for the set duration of time, it stops recording and sends it to the Whisper API for transcription. After it's transcribed, the text is sent to the OpenAI API (via OpenAIDemo.cs) for a completion which is then sent to the ElevenLabs API (via ELSpeaker.cs) for the generation of the voice and played.
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using OpenAi.Api.V1;
using OpenAi.Unity.V1;
using OpenAI.Integrations.VoiceRecorder;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using OpenAI.DemoScript;
using OpenAI.Integrations.ElevenLabs;

namespace EqualReality.ReviewAI.GPTAIIntergration
{
	[System.Serializable]
	public class Utterence
	{
		public bool isReadyForGPT
		{
			get{
				if(voiceToText == null)
					return false;
					
				return true;
			}
		}
		public bool isReadyForVoice
		{
			get{
				if(responseText == null)
					return false;
					
				return true;
			}
		}
		public string voiceToText = null;
		public string responseText = null;
		
		
	}
	
	[System.Serializable]
	public class VoiceMessage
	{
		
		public List<Utterence> utterence = new List<Utterence>();
		
		/*public bool isReady {
			get{
				for(int i = 0; i < utterence.Count; i++)
					if(!utterence[i].isReady)
						return false;
						
				return true;
				
			}
		}*/
		
		public string Output {
			get{
				
				string output= "";
				
				for(int i = 0; i < utterence.Count; i++)
					output+= utterence[i].voiceToText;
				
				return output;
			}
		}
	}
	
    [RequireComponent(typeof(AudioSource))]
	public class VoiceRecorder : OpenAI.Integrations.ElevenLabs.VoiceRecorder
    {
        private AudioSource audioSource; // The first audio source is responsible for playback
        private AudioSource micMonitor; // The second audio source is responsible for monitoring the mic

	    //public int recordingLength = 10; // Length of recording in seconds
	    //public float recordingThreshold = 0.8f; // Mic volume threshold for triggering recording
	    public float recordingTimeUtterancesThreshold = 1.5f; // Time threshold for stopping recording after mic volume drops below threshold
	    //public float recordingTimeThreshold = 2.5f; // Time threshold for stopping recording after mic volume drops below threshold
	    //public float sensitivity = 100.0f; // Used to adjust the sensitivity of the mic volume
	    //public float loudness = 0.0f; // Current volume level of the microphone
	    private bool isRecording = false; // Used to make sure we don't start recording multiple times
        private bool processingResponse = false; // Used to make sure we don't start processing multiple responses
	    private float timeBelowThreshold = 0.0f, timeBelowUtterancesThreshold; // Used to keep track of how long the mic volume has been below the threshold

	    public List<VoiceMessage> voiceMessages = new List<VoiceMessage>();
	    public int vmId = 0;

	    float utterance = 0;
	    
	    public UnityEvent onMicMonitor, onMicRecording, onUtterence, onMicStopped;

	    public bool onStart = false;

        void Start()
	    {
	    	if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
	    	{
		    	Application.RequestUserAuthorization(UserAuthorization.Microphone);
	    	}
	    	
		    voiceMessages.Add(new VoiceMessage());
		    
		    if(onStart)
            	MonitorMic();
	    }
        
	    public void ResetListerning()
	    {
	    	voiceMessages.Clear();
		    voiceMessages.Add(new VoiceMessage());
	    }
	    

        void Update()
        {
            // Get the current volume level
            ELSpeaker ELSpeaker = GetComponent<ELSpeaker>();
            if (!ELSpeaker.responsePlaying)
            {
                loudness = GetAveragedVolume() * sensitivity;
            }

            // Check if mic volume is above threshold and start recording if it is
            if (!isRecording && !processingResponse && loudness > recordingThreshold)
            {
                StartRecording();
	            isRecording = true;
	            utterance = 0;
            }
            // Check if mic volume is below threshold and stop recording if it is for a certain amount of time
            else if (isRecording && loudness < recordingThreshold)
            {
	            timeBelowThreshold += Time.deltaTime;
	            timeBelowUtterancesThreshold += Time.deltaTime;
	            
	            //Debug.Log("Time below threshold: " + timeBelowThreshold + " Loudness: " + loudness + " is less than " + recordingThreshold);
	            if (timeBelowUtterancesThreshold >= recordingTimeUtterancesThreshold)
	            {
		            ProcessUtteranceRecording();
		            //isRecording = false;
		            processingResponse = true;
		            timeBelowUtterancesThreshold = 0.0f;
		            loudness = 0.0f;
		            Debug.Log("Still recording, processing Utterances...");
		            StartRecording();
	            }
	            
	            //Debug.Log("Time below threshold: " + timeBelowThreshold + " Loudness: " + loudness + " is less than " + recordingThreshold);
	            if (timeBelowThreshold >= recordingTimeThreshold)
	            {
		            StopRecording();
		            isRecording = false;
		            processingResponse = true;
		            timeBelowThreshold = 0.0f;
		            timeBelowUtterancesThreshold = 0.0f;
		            loudness = 0.0f;
		            Debug.Log("Recording has stopped, processing response...");
	            }
            }
            // Reset time below threshold if mic volume goes back above threshold
            else if (isRecording && loudness > recordingThreshold)
            {
	            timeBelowThreshold = 0.0f;
	            timeBelowUtterancesThreshold = 0.0f;
            }
        }
        
	    public string micDeviceName;

        //Listen to the microphone
	    public void MonitorMic()
	    {
		    onMicMonitor.Invoke();
		    
		    if(audioSource == null)
			    audioSource = GetComponent<AudioSource>();
		    else
		    {
		    	DestroyImmediate(audioSource);
		    	audioSource = gameObject.AddComponent<AudioSource>();
		    }
			    
            Debug.Log("Audio source 1 muted");
            audioSource.volume = 0; // Mute the first audio source to prevent mic monitoring
		    if(micMonitor == null)
			    micMonitor = GetComponents<AudioSource>()[1];
		    else
		    {
		    	DestroyImmediate(micMonitor);
		    	micMonitor = gameObject.AddComponent<AudioSource>();
		    }
            Debug.Log("Audio source 2 volume set to 1");
            micMonitor.volume = 1; // Set the volume of the second audio source to 1 to monitor the mic
		    micDeviceName = Microphone.devices[0];
		    //GetComponent<AudioSource>().clip = Microphone.Start(micDeviceName, true, recordingLength, 16000);
		    //GetComponent<AudioSource>().loop = true;
		    
		    audioSource.clip = Microphone.Start(micDeviceName, true, recordingLength, 16000);
		    audioSource.loop = true;
            
		    while (!(Microphone.GetPosition(null) > 0)) { }
            audioSource.Play();
        }

        // Gets the current volume level
        float GetAveragedVolume()
        {
            float[] data = new float[1024];
            int micPosition = Microphone.GetPosition(null) - (1024 + 1); // Get the position 1024 samples ago
            if (micPosition < 0)
                return 0; // Return 0 if the position is negative
            GetComponent<AudioSource>().clip.GetData(data, micPosition);
            float a = 0;
            foreach (float s in data)
            {
                a += Mathf.Abs(s);
            }
            return a / 1024;
        }

        public void StartRecording()
        {
            if (Microphone.devices.Length == 0)
            {
                Debug.LogWarning("No microphone found to record audio clip sample with.");
                return;
            }
            
	        onMicRecording.Invoke();

            string mic = Microphone.devices[0];
            Debug.Log("Microphone is recording...");
            audioSource.clip = Microphone.Start(mic, false, recordingLength, 16000);
            //GetComponent<AudioSource>().loop = true;
            while (!(Microphone.GetPosition(null) > 0)) { }
            audioSource.Play();
        }

	    public async void ProcessUtteranceRecording()
	    {
	    	onUtterence.Invoke();
		    Debug.Log("Processing Utterances...");
		    //int recordingLength = Microphone.GetPosition(null) / 16000; // Calculate the actual length of the recording in seconds
		    Microphone.End(null);
		    string filepath;
		    WavUtility.FromAudioClip(audioSource.clip, out filepath);
		    Debug.Log($"Voice recording saved to: {filepath}");
		    GetComponent<AudioSource>().Stop();
		    // SavWav.Save(fileName, audioSource.clip);
		    
	    	utterance++;
	    	
		    await TranscriptRecording();
		    processingResponse = false;
		    Debug.Log("Response processing complete, listening for new mic input...");
		    //MonitorMic();
	    }

        public async void StopRecording()
	    {
		    onMicStopped.Invoke();
		    
            //int recordingLength = Microphone.GetPosition(null) / 16000; // Calculate the actual length of the recording in seconds
            Microphone.End(null);
            string filepath;
            WavUtility.FromAudioClip(audioSource.clip, out filepath);
            Debug.Log($"Voice recording saved to: {filepath}");
            audioSource.Stop();
            // SavWav.Save(fileName, audioSource.clip);
	        await PlayTranscriptRecording();
            processingResponse = false;
	        Debug.Log("Response processing complete");//, listening for new mic input...");
	        //MonitorMic();
        }

        public void PlayRecording()
        {
	        audioSource.Play();
        }
        
        

	    public async Task PlayTranscriptRecording()
	    {
		    Debug.Log("waiting for response text...");
	    	
	    	
		    ELSpeaker ELSpeaker = GetComponent<ELSpeaker>();
		    
		    for(int  i = 0; i < voiceMessages[vmId].utterence.Count; i++)
		    {
		    	while(!voiceMessages[vmId].utterence[i].isReadyForVoice)
		    	{
			    	await Task.Yield();
		    	}
		    	
			    Debug.Log("Playing response audio...");
			    ELSpeaker.SpeakSentence(voiceMessages[vmId].utterence[i].responseText);
			    
			    while (ELSpeaker.responsePlaying)
			    {
				    await Task.Yield();
			    }
		    }
	            
		    if(gameObject == null)
			    return;
			    
	    	GetComponent<AudioSource>().volume = 1;
		    
		    // Check if the ELSpeaker is playing and wait until it stops playing
		    while (ELSpeaker.responsePlaying)
		    {
			    await Task.Yield();
		    }
	    

	    }
	    
	    public async Task TranscriptRecording()
	    {
	    	Utterence utterence = new Utterence();
	    	voiceMessages[vmId].utterence.Add(utterence);
	    	
            var transcript = await SendTranscriptRequest(
                audioSource.clip, // The audio file to be transcribed
                "Hello, welcome to my lecture." // For details on Whisper API prompting, see: https://platform.openai.com/docs/guides/speech-to-text/prompting
            );
            if (transcript.IsSuccess)
            {
            	if(input != null) input.text = transcript.Result.text;
            	Debug.Log(transcript.Result.text);
	            utterence.voiceToText = transcript.Result.text;
	            OpenAIDemo openAIDemo = GetComponent<OpenAIDemo>();
	            await openAIDemo.SendOpenAIRequest(transcript.Result.text);
	            utterence.responseText = openAIDemo.messages[openAIDemo.messages.Count-1].content;//response.text;
            }
            else
            {
                input.text =
                    $"ERROR: StatusCode: {transcript.HttpResponse.responseCode} - {transcript.HttpResponse.error}";
            }
        }

        public async Task<ApiResult<TranscriptionV1>> SendTranscriptRequest(
            AudioClip clip,
            string prompt
        )
        {
            SOAuthArgsV1 auth = completer.Auth;
            OpenAiApiV1 api = new OpenAiApiV1(auth.ResolveAuth());
            Debug.Log("Sending transcript request to the Whisper API...");
            //Convert voice from clip to wav
            string filepath;
            byte[] audioFile = WavUtility.FromAudioClip(audioSource.clip, out filepath, false);

            ApiResult<TranscriptionV1> comp =
                await api.Audio.Transcriptions.CreateTranscriptionAsync(
                    new TranscriptionRequestV1()
                    {
                        model = "whisper-1",
                        prompt = prompt,
                        response_format = "json",
                        audioFile = audioFile
                    }
                );

            return comp;
        }
    }
}

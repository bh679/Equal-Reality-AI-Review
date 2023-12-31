﻿using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.IO;
using NAudio.Wave;
using OpenAi.Unity.V1;
using TMPro;
using OpenAI.Integrations.ElevenLabs.Configuration;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using OpenAI.Integrations.VoiceRecorder;

namespace EqualReality.ReviewAI.GPTAIIntergration
{
	public class ELSpeaker : OpenAI.Integrations.VoiceRecorder.ELSpeaker
    {
	    
        private const string url = "https://api.elevenlabs.io/v1/text-to-speech/21m00Tcm4TlvDq8ikWAM";
        private readonly WaveOutEvent outputDevice = new WaveOutEvent();
        
        
        private const string jsonBodyTemplate =
            "{\"text\": \"{{text}}\", \"voice_settings\": {\"stability\": {{stability}}, \"similarity_boost\": {{similarity_boost}}}}";

	    public UnityEvent onSendForVoice, onDownloadingVoice, onPlayingVoice, onFinishPlayingVoice;

	    
        public void SpeakSentenceFromInput()
        {
            responsePlaying = true;
            Debug.Log("Speaking: " + textInput.text);
            StartCoroutine(_SpeakSentence(textInput.text));
        }
        
	    public void SpeakSentence(string input)
	    {
		    responsePlaying = true;
		    Debug.Log("Speaking: " + input);
		    StartCoroutine(_SpeakSentence(input));
	    }
	    
	    public IEnumerator _SpeakSentence(string input)
	    {
	    	onSendForVoice.Invoke();
	    	
		    Debug.Log("SpeakSentence");
            var APIkey = ELAuth.PrivateApiKey;
            string jsonBody = jsonBodyTemplate
                .Replace("{{text}}", input)
                .Replace("{{stability}}", stability.ToString(CultureInfo.InvariantCulture))
                .Replace(
                    "{{similarity_boost}}",
                    similarity_boost.ToString(CultureInfo.InvariantCulture)
                );
            Debug.Log("json body is: " + jsonBody);
            // Create request content
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            UploadHandlerRaw body = new UploadHandlerRaw(bodyRaw);
            body.contentType = "application/json";

		    onDownloadingVoice.Invoke();
            using (UnityWebRequest www = UnityWebRequest.Post(url, ""))
            {
                www.SetRequestHeader("Content-Type", "application/json");
                www.SetRequestHeader("accept", "audio/mpeg");
                www.SetRequestHeader("xi-api-key", APIkey);

                www.uploadHandler = body;
                www.downloadHandler = new DownloadHandlerAudioClip(url, AudioType.MPEG);
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
	                Debug.LogError($"Audio file download error: {www.error}\nIt Could be that you maxed your subscription."+input);
	                failedCalls.Add(input);
                    yield break;
                }

                if (useAudioStreaming)
                {
                    Debug.Log("Starting audio stream");
                    // Play an audio stream, which allows you to play the audio without downloading it. Not supported on all platforms. See: https://docs.elevenlabs.io/guides/text-to-speech
                    var audioStream = new MemoryStream(www.downloadHandler.data);
                    var audioReader = new Mp3FileReader(audioStream);
                    outputDevice.Init(audioReader);
                    outputDevice.Play();

                    // Wait for the audio stream to finish playing
                    while (audioReader.CurrentTime < audioReader.TotalTime)
                    {
                        yield return null;
                    }
                    Debug.Log("Audio stream is done playing");
                    responsePlaying = false;
                }
                else
                {
                	
                    Debug.Log("Downloading audio file");
                    // Play the downloaded audio file. This is the easiest way to play audio, but it requires downloading the entire file before playing it.
                    AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
                    AudioSource audioSource = GetComponent<AudioSource>();
                    audioSource.clip = audioClip;
                    audioSource.loop = false;
                    audioSource.Play();
                    onPlayingVoice.Invoke();
                    audioSource.volume = 1f;
                    Debug.Log($"Audio response is {audioSource.clip.length} seconds long");
                    
                    // Check if the audio source is playing and wait until it stops playing
                    while (audioSource.isPlaying)
                    {
                        yield return null;
                    }
                    Debug.Log("Downloaded Audio is done playing");
	                responsePlaying = false;
                    
                    OnApplicationQuit();
	                audioSource.clip = null;
                    
                    onFinishPlayingVoice.Invoke();
	                yield break;
	                
                    }
                    
            }
                
	    }
            
	    List<string> failedCalls = new List<string>();
	    void Update()
	    {
	    	if(failedCalls.Count > 0)
	    	{
	    		StartCoroutine(_SpeakSentence(failedCalls[0]));
	    		failedCalls.RemoveAt(0);
	    	}
	    }
            

        private void OnApplicationQuit()
        {
            // Stop the player from exiting the app
            outputDevice.Stop();
            outputDevice.Dispose();
        }
    }
}

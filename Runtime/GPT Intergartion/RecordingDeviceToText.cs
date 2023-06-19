using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI.Integrations.ElevenLabs;

public class RecordingDeviceToText : MonoBehaviour
{
	public VoiceRecorder vr;
	public TMPro.TMP_Text text;
	public AudioSource source;

	float y = 0;
	// Update is called once per frame
	void Update()
	{
		text.text = "Access: " + Application.HasUserAuthorization(UserAuthorization.Microphone) + "\n"
			+vr.micDeviceName + "\n"
			+(source == null?"":source.clip.name);
		
		
	}
}

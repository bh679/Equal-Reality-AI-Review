using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI.Integrations.ElevenLabs;

public class LoudnessToHeight : MonoBehaviour
{
	public VoiceRecorder_Ext vr;

	float y = 0;
	// Update is called once per frame
	void Update()
	{
		
		y = vr.loudness;
		
		this.transform.localScale = new Vector3(1,y,1);
		
		
	}
}

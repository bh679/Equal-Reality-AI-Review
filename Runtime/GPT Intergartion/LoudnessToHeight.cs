using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EqualReality.ReviewAI.GPTAIIntergration
{

	public class LoudnessToHeight : MonoBehaviour
	{
		public VoiceRecorder vr;
		public float multiplier = 1;
	
		float y = 0;
		// Update is called once per frame
		void Update()
		{
			
			y = vr.loudness;
			
			this.transform.localScale = new Vector3(1,y * multiplier,1);
			
			
		}
	}

}
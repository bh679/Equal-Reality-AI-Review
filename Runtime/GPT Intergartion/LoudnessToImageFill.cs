using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EqualReality.ReviewAI.GPTAIIntergration
{

	public class LoudnessToImageFill : MonoBehaviour
	{
		public VoiceRecorder vr;
		public float multiplier = 1;
		float y = 0;
		
		public Image image;
		
		public bool scaleWhenBeyondOne = true;
	
		// Update is called once per frame
		void Update()
		{
			
			y = vr.loudness * multiplier;
			
			image.fillAmount = y;
			
			if(scaleWhenBeyondOne && y > 1)
				this.transform.localScale = new Vector3(y,y,y);
			else
				this.transform.localScale = Vector3.one;
		}
	}

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EqualReality.ReviewAI.GPTAIIntergration
{
	public class LoudnessEvent : MonoBehaviour
	{
		public float minimum = 0.9f;
		
		public VoiceRecorder vr;
		
		public UnityEvent onToLoud, onNowGood;
		bool isToLoud = false;
		
		
	    // Start is called before the first frame update
	    void Start()
	    {
		    isToLoud = !(vr.loudness > minimum);
	    }
	
	    // Update is called once per frame
	    void Update()
		{
			if((vr.loudness > minimum) != isToLoud)
		    {
				if(isToLoud)
					onNowGood.Invoke();
				else
					onToLoud.Invoke();
		    }
		    
		    
		    isToLoud = (vr.loudness > minimum);
	    }
	}
}
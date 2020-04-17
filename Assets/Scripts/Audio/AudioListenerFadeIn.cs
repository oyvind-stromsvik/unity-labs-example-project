using UnityEngine;

public class AudioListenerFadeIn : MonoBehaviour
{
	int fadeSamples = 0;
	int fadeCounter = 0;


    void OnAudioFilterRead(float[] data, int channels)
	{
        // Check whether the incoming data is actually silent or not.
		bool silent = true;
		for(int n = 0; n < data.Length; n++)
			if(Mathf.Abs(data[n]) > 0.001f)
				silent = false;

        // First time through when non-silent set samples and counter based the sample rate
		if(fadeSamples == 0 && !silent)
		{
            fadeSamples = 2 * 48000;
            fadeCounter = -2 * 48000;
		}

        // If the counter hasn't exceeded the samples yet multiply the data by a value based on the counter over the samples.
		if(fadeCounter < fadeSamples)
		{
			float scale = 1.0f / fadeSamples;
			for(int n = 0; n < data.Length; n++)
				data[n] *= Mathf.Clamp((float)(fadeCounter++) * scale, 0.0f, 1.0f);
		}
	}
}

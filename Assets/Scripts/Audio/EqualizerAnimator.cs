using System;
using UnityEngine;

public class EqualizerAnimator : MonoBehaviour
{
    // Will update with comments in future.
    [Serializable]
	public class Filter
	{
		public float cut, bw, falloff;

		public float lpf = 0.0f;
		public float bpf = 0.0f;
		public float env = 0.0f;
		public float gain = 1.0f;
		
		public float Process(float input)
		{
			lpf += cut * bpf;
			float hpf = input - lpf - bpf * bw;
			bpf += cut * hpf;
			float output = bpf * gain;
			float a = output * output;
			if(a > env)
				env = a;
			else
				env *= falloff;
			return env;
		}
	}


    [Serializable]
    public class Advanced
    {
        [Range(0.0001f, 0.01f)]
        public float cutOffset = 0.003f;
        public int Monitor = 0;
    }


    [Range(0.1f, 1f)]
    public float scaleProportion = 0.6f;
    public float minimumHeight = 0.2f;
    public float speed = 10f;
    public Transform[] topEqualizerColumns;
    public Transform[] bottomEqualizerColumns;
    public Advanced advanced;
    public float normalizeGain = 0.0f;
	public float envClipping = 5.0f;

	private float env = 1.0f;
    private Filter[] bands;
	private bool initialized;
    private Renderer[] renderers;

	
	void Start ()
    {

        if (topEqualizerColumns.Length != bottomEqualizerColumns.Length)
        {
            Debug.LogError("Top and Bottom equalizers are not equal!");
            return;
        }

        if (topEqualizerColumns.Length == 0)
        {
            Debug.LogError("No equalizer columns assigned");
            return;
        }
        bands = new Filter[topEqualizerColumns.Length];
		for(int i = 0; i < bands.Length; i++)
		{
			bands[i] = new Filter();
			bands[i].cut = Mathf.Pow((float)(i + 1) / (float)bands.Length, 3.0f) + advanced.cutOffset;
			bands[i].bw = 1.0f / (float)bands.Length;
			bands[i].falloff = 0.9998f;
			bands[i].gain = 1.0f - bands[i].cut - bands[i].bw;
        }

		initialized = true;
	}

	
	void Update ()
	{
        if (!initialized)
            return;

        for (int i = 0; i < topEqualizerColumns.Length; i++)
        {
            if (topEqualizerColumns[i] == null || bottomEqualizerColumns[i] == null)
                continue;

            Vector3 newScale = topEqualizerColumns[i].localScale;
			newScale.y = Mathf.Lerp(newScale.y, Mathf.Clamp(bands[i].env, 0.0f, envClipping), speed * Time.deltaTime) * scaleProportion + minimumHeight;
            topEqualizerColumns[i].localScale = newScale;
            bottomEqualizerColumns[i].localScale = newScale;
        }
	}

	
	void OnAudioFilterRead(float[] data, int numchannels)
	{
		if(!initialized)
			return;
		for(int n = 0; n < data.Length; n += numchannels)
		{
			float sum = 0.0f;
			for(int i = 0; i < numchannels; i++)
				sum += data[n + i];
			if(normalizeGain > 0.0f)
			{
				env += (Mathf.Abs(sum) - env) * 0.003f;
				sum *= normalizeGain / (env + 0.001f);
			}
			for(int i = 0; i < bands.Length; i++)
				bands[i].Process(sum);
			if(advanced.Monitor > 0)
			{
				float m = bands[advanced.Monitor - 1].bpf;
				for(int i = 0; i < numchannels; i++)
					data[n + i] = m;
			}
		}
	}
}
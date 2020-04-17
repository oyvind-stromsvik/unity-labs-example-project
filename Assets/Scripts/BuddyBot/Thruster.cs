using System.Linq;
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Thruster : MonoBehaviour
{

	[Range(0, 1)] public float Level;

    public ParticleSystem[] Systems;

    private float[] m_StartingSizes;
    private float[] m_StartingLifetimes;
    private float m_StartingIntensity;

    public void OnEnable()
    {
        m_StartingSizes = Systems.Select(s => s.startSize).ToArray();
        m_StartingLifetimes = Systems.Select(s => s.startLifetime).ToArray();
    }

	public void OnDisable()
	{
		SetLevel (1);
	}

	private void SetLevel(float level)
	{
		level = Mathf.Clamp01 (level);
		for (int i = 0; i < Systems.Length; ++i)
		{
			Systems[i].startSize = m_StartingSizes[i]*level;
			Systems[i].startLifetime = m_StartingLifetimes[i]*level;
		}
	}

    public void Update()
    {
		SetLevel (Level);
    }
}

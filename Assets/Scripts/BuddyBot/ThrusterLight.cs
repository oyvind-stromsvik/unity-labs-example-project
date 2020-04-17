using System.Linq;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class ThrusterLight : MonoBehaviour
{

    public Thruster[] Thrusters;
    public float BaseThrust = 1f;
    public float BaseIntensity = 1f;
    public float IntensityChangePerSec = 10f;
    private Light m_Light;

    private float m_TargetIntensity;

    public void Awake()
    {
        m_Light = GetComponent<Light>();
        m_Light.enabled = true;
    }

    public void LateUpdate()
    {
        float thrusterLevel = 0;
        for (int i = 0; i < Thrusters.Length; ++i)
            thrusterLevel += Thrusters[i].Level;

        m_TargetIntensity = BaseIntensity*thrusterLevel/BaseThrust;
        m_Light.intensity = Mathf.MoveTowards(m_Light.intensity, m_TargetIntensity, Time.deltaTime*IntensityChangePerSec);
    }
}

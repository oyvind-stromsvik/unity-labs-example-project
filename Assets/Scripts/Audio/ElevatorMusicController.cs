using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class ElevatorMusicController : MonoBehaviour
{
    public AudioMixer mixer;                        // The mixer that controls the elevator music.
    public AudioMixerSnapshot doorsClosedSnapshot;  // Snapshots for when the doors are open and closed.
    public AudioMixerSnapshot doorsOpenSnapshot;


    private ElevatorDoor[] m_ElevatorDoors;         // All of the elevator doors.
    private Elevator[] m_Elevators;                 // All of the elevators.
    private AudioMixerSnapshot[] m_Snapshots;       // Array of the public snapshots, used to transition to.
    private float[] m_Weights;                      // Weights of the snapshots.


    void Awake()
    {
        m_ElevatorDoors = FindObjectsOfType<ElevatorDoor>();
        m_Elevators = FindObjectsOfType<Elevator>();

        m_Snapshots = new[] {doorsClosedSnapshot, doorsOpenSnapshot};
        m_Weights = new float[2];
    }


    void Update()
    {
        // If the player is in the lift, the is no occlusion, otherwise full occlude.
        float occlusion = (m_Elevators.Any(e => e.IsPlayerInElevator)) ? 0 : 1;

        // Find the minimum occlusion based on the openness of all the elevator doors.
        for (int i = 0; i < m_ElevatorDoors.Length && occlusion > 0; ++i)
            occlusion = Mathf.Min(occlusion, 1 - m_ElevatorDoors[i].Openness);

        // Weights are calculated based on the occlusion.
        m_Weights[0] = occlusion;
        m_Weights[1] = 1 - occlusion;

        mixer.TransitionToSnapshots(m_Snapshots, m_Weights, 0);
    }
}

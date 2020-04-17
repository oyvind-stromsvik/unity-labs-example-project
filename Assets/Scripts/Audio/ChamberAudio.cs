using UnityEngine;
using UnityEngine.Audio;

public class ChamberAudio : MonoBehaviour
{
	public AudioMixer mixer;                                // Reference to the mixer that controls the music for the chamber.
	public AudioMixerSnapshot insideDoorOpenSnapshot;       // References to the snapshots for difference scenarios.
	public AudioMixerSnapshot insideDoorClosedSnapshot;
	public AudioMixerSnapshot outsideDoorOpenSnapshot;
	public AudioMixerSnapshot outsideDoorClosedSnapshot;
    public float minDistance;                               // Distance below which the audio isn't occluded.
    public float maxDistance;                               // Distance above which the audio is fully occluded.
    public float coneAngle;                                 // The angle through which the audio is played fully.
    public float ceilingHeight;                             // The height of the chamber.
    public DoorManager door;                                // Reference to the door to the chamber.


    private Vector3 m_TopDownCameraPosition;                // The position of the audio listener not taking vertical position into account.
    private float m_CurrentDistance;                        // Horizontal distance between the audio source and the audio listener.
    private float m_CurrentAngle;                           // Angle between the source's forward and the listener.
    private float m_Openness;                               // How open the door is.
    private float m_OcclusionFactor;                        // How much the distance, angle and openness affect the weight of the snapshots.
    private AudioListener m_Listener;                       // Reference to the listener.
    private AudioMixerSnapshot[] m_Snapshots;               // An array of the public snapshots, used for transitioning to.
    private float[] m_Weights;                              // An array of the weights of the snapshots.


    void Start()
    {
        // Set up the arrays for transitioning.
        m_Snapshots = new[] {
			insideDoorOpenSnapshot,
			insideDoorClosedSnapshot,
			outsideDoorOpenSnapshot,
			outsideDoorClosedSnapshot
		};

        m_Weights = new float[4];
    }


	void Update()
	{
        // Only continue if there is an audio listener.
	    if (!m_Listener)
	        m_Listener = FindObjectOfType<AudioListener>();

	    if (!m_Listener)
            return;

        // Get the position of the listener and if it's above the ceiling, return.
	    m_TopDownCameraPosition = m_Listener.transform.position;
	    if (m_TopDownCameraPosition.y > ceilingHeight) return;

        // Level the listener position with the source.
	    m_TopDownCameraPosition.y = transform.position.y;

        // Calculate the distance and angle using the listener's horizontal position.
	    m_CurrentDistance = Vector3.Distance(transform.position, m_TopDownCameraPosition);
	    m_CurrentAngle = Vector3.Angle(transform.forward, m_TopDownCameraPosition - transform.position);

        m_Openness = door.Openness;

        // If the distance is below minimum, don't occlude.
	    if (m_CurrentDistance < minDistance)
	        m_OcclusionFactor = 0;
        // If the door isn't open, occlude completely.
	    else if (m_Openness <= 0)
	        m_OcclusionFactor = 1;
        // Otherwise base the occlusion on the angle between the source the the listener and the openness of the door.
	    else m_OcclusionFactor = Mathf.Clamp01(m_CurrentAngle/(coneAngle*m_Openness));

        // Find the relative distance of the source and listener.
	    float distanceValue = Mathf.Clamp01((m_CurrentDistance - minDistance)/(maxDistance - minDistance));

        // Calculate the weights based on the relative distance and the occlusion.
	    m_Weights[0] = (1f - distanceValue)*(1 - m_OcclusionFactor)*m_Openness;
	    m_Weights[1] = (1f - distanceValue)*(1 - m_OcclusionFactor)*(1 - m_Openness);
	    m_Weights[2] = distanceValue*(1 - m_OcclusionFactor);
	    m_Weights[3] = distanceValue*m_OcclusionFactor;

        // Transition to the snapshots based on the calculated weights.
		mixer.TransitionToSnapshots (m_Snapshots, m_Weights, 0);
	}
}

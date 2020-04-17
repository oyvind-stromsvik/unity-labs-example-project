using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class DoorManager : MonoBehaviour 
{
    public AudioClip AudioOpenDoorClip;         // Clip to be played when the door opens.
    public AudioClip AudioCloseDoorClip;        // Clip to be played when the door closes.
    public float StayOpenForTime = 0.5f;        // How long the door will stay open for before it closes if it can.

    
	public UnityEvent OnBeginOpening;           // Called when the door starts openning.
	public UnityEvent OnBeginClosing;           // Called when the door starts closing.


	private float m_LastOpenAtTime;             // Recorded for timing how long the door should be open.
    private Animator m_Anim;                    // Reference to the animator to control the door openning and closing.


    private readonly int m_HashOpennessPara = Animator.StringToHash ("Openness");   // For referencing the animator parameter.
    private readonly int m_HashDoorOpenPara = Animator.StringToHash ("DoorOpen");   // For referencing the animator parameter.


    public float Openness { get { return m_Anim.GetFloat(m_HashOpennessPara); } }   // Used to control audio levels.


    protected virtual bool ShouldBeOpen { get { return m_LastOpenAtTime > (Time.time - StayOpenForTime); } }    // For controlling timing on door opening.
    protected bool IsOpen { get { return Openness > 0; } }


	void Awake ()
    {
		m_Anim = GetComponent<Animator> ();
		m_LastOpenAtTime = -10f;
	}


    void Start ()
    {
        // Initialise all the SMBs on the door's animator.
        CustomSMB[] allSMBs = m_Anim.GetBehaviours<CustomSMB> ();
        for (int i = 0; i < allSMBs.Length; i++)
        {
            allSMBs[i].Init (m_Anim);
        }
    }

	
	protected virtual bool ShouldOpenForCollider(Collider col)
	{
        // Doors should open for the player or the buddy bot.
		return col.CompareTag ("Player") || col.CompareTag ("BuddyBot");
	}


	void OnTriggerStay(Collider col)
	{
	    if(!enabled) return;

		if (!ShouldOpenForCollider(col)) return;

        // If it's the player or the buddy bot in the trigger, record the open time.
		m_LastOpenAtTime = Time.time;
	}
	
		
	public virtual void Update()
	{
		m_Anim.SetBool (m_HashDoorOpenPara, ShouldBeOpen);
	}


}

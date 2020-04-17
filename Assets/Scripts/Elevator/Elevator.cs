using UnityEngine;
using System.Collections;

public class Elevator : MonoBehaviour
{
    public float speed = 3f;                        // How fast the elevator moves.
    public float doorClosingDelay = 1f;             // How long the elevator waits for the doors to close.
    public Animator groundFloorSign;                // Animator for the sign outside the elevator on the ground floor.
    public Animator firstFloorSign;                 // Animator for the sign outside the elevator on the first floor.
	public Animator elevatorShaft;                  // Animator for the sign inside the elevator.
	public float elevatorFloorOffset = 0.01f;       // Offset from the floors to stop z-fighting.


    public int CurrentFloor { get; private set; }
    public bool ElevatorMoving { get; private set; }
    public bool IsPlayerInElevator { get { return m_PlayerRb != null; } }


    private Rigidbody m_Rigidbody;                      // Reference used to move the elevator.
    private Rigidbody m_PlayerRb;                       // Reference used to move the player.
    private FootPlantingPlayer m_FootPlanting;          // Reference used to prevent the player juddering in the elevator.
    private AudioSource m_AudioSource;                  // Used to play audio when the lift moves.
    private WaitForSeconds m_CloseDoorsDelayTimer;      // Timer for the doorClosingDelay.
    private Vector3 m_ReferencePos;                     // Reference to measure other positions from.


    private readonly int m_HashLiftMovingPara = Animator.StringToHash ("LiftMoving");   // For referencing the animator parameter.
    private readonly int m_HashShowSignPara = Animator.StringToHash ("ShowSign");       // For referencing the animator parameter.


    private const float k_FloorToYConversionRate = 6f;  // The difference in meters between floors.


    void Awake ()
    {
        // Find references
        m_Rigidbody = GetComponent<Rigidbody> ();
        m_AudioSource = GetComponent<AudioSource> ();

        // Create the timer.
        m_CloseDoorsDelayTimer = new WaitForSeconds (doorClosingDelay);

        // Start on the first floor.
        CurrentFloor = 1;

        // Set the initial position as a reference for determining the other floor positions.
        m_ReferencePos = transform.position;
    }


    public void TeleportToFloor(int floorNum)
    {
        // Move the lift directly to a required position.
        transform.position = GetTargetPosForFloor(floorNum);

        // Set the current floor accordingly.
        CurrentFloor = floorNum;

        // The elevator is not moving.
        elevatorShaft.SetBool(m_HashLiftMovingPara, false);
        ElevatorMoving = false;
    }


    void OnTriggerEnter (Collider other)
    {
        // Exit if it's not the player or the player is already in the elevator.
        if(other.tag != "Player" || IsPlayerInElevator)
            return;

        // Find references on the player for moving it and stopping it from juddering.
        m_PlayerRb = other.GetComponent<Rigidbody> ();
		m_FootPlanting = other.GetComponent<FootPlantingPlayer> ();

        TravelInElevator ();
    }


    void OnTriggerExit (Collider other)
    {
        // If it's not the player leaving then quit.
        if (other.tag != "Player")
            return;

        // Reset the references.
        m_PlayerRb = null;
        m_FootPlanting = null;
    }


    IEnumerator MoveElevator (int targetFloor)
    {
        // If the elevator is already at the target floor or is already moving, exit.
        if (CurrentFloor == targetFloor || ElevatorMoving) yield break;

        // The elevator is now moving and it's target position is on the target floor.
        ElevatorMoving = true;

        // If the player is in the lift and we have a reference to it's foot planting, stop damping to prevent juddering.
		if(m_FootPlanting != null)
        	m_FootPlanting.useBodyPositionDamping = false;

        // Wait for the doors to close.
        yield return m_CloseDoorsDelayTimer;

        // Find a position based on the target floor.
        var targetPos = GetTargetPosForFloor(targetFloor);

        // Tell the animator the lift is now moving.
        elevatorShaft.SetBool(m_HashLiftMovingPara, true);

        // While the elevator is not near it's target position...
        while (Mathf.Abs (m_Rigidbody.position.y - targetPos.y) > float.Epsilon)
        {
            if(!m_AudioSource.isPlaying)
                m_AudioSource.Play();

            // Move the elevator.
            m_Rigidbody.position = Vector3.MoveTowards (m_Rigidbody.position, targetPos, speed * Time.deltaTime);

            // Move the player with the elevator.
            if (m_PlayerRb != null)
            {
                Vector3 playerTargetPos = targetPos + m_PlayerRb.position - m_Rigidbody.position;
                m_PlayerRb.position = Vector3.MoveTowards (m_PlayerRb.position, playerTargetPos, speed * Time.deltaTime);
            }

            // Loop on the next frame.
            yield return null;
        }

        // Tell the animator the lift is no longer moving.
        elevatorShaft.SetBool(m_HashLiftMovingPara, false);

        // The elevator is no longer moving and has reached it's target floor.
        ElevatorMoving = false;

        // Reactivate body damping.
		if(m_FootPlanting != null)
        	m_FootPlanting.useBodyPositionDamping = true;

        CurrentFloor = targetFloor;
    }

    private Vector3 GetTargetPosForFloor(int targetFloor)
    {
        // Return the required position of the elevator based on the floor it's going to.
        Vector3 targetPos = m_ReferencePos;
        targetPos.y = targetFloor*k_FloorToYConversionRate+elevatorFloorOffset;
        return targetPos;
    }


    public void CallElevator (int targetFloor)
    {
        // If the elevator isn't on the required floor.
        if (CurrentFloor != targetFloor)
        {
            // Activate the sign on the correct floor.
            if(targetFloor == 0)
                groundFloorSign.SetTrigger(m_HashShowSignPara);
            else
                firstFloorSign.SetTrigger(m_HashShowSignPara);
            
            // Move the elevator to the target floor.
            StartCoroutine (MoveElevator (targetFloor));
        }
    }


    void TravelInElevator ()
    {
        // Move the elevator to the opposite of it's current floor.
        if (CurrentFloor == 1)
            StartCoroutine (MoveElevator (0));
        else
            StartCoroutine (MoveElevator (1));
    }
}

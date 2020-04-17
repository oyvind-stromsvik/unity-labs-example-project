using UnityEngine;

public class SetupAndUserInput : MonoBehaviour
{
    // Properties for the different inputs.
    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }
    public float SlowMo { get; private set; }

    public bool InputEnabled = true;                    // Used to turn off input when on fly through.
    public float LastInputAt { get; private set; }      // To store the time of the last input detection.

    public bool JumpDown                                // If the jump button is just been pressed.
    {
        get
        {
            return m_JumpDown;
        }
        private set { m_JumpDown = value; }
    }

    
    public Transform cameraRig;         // Reference to the camera rig in the scene so that SMBs have access to it.
    public Transform mainCamera;        // Reference to the camera itself.


    private Animator m_Animator;        // Reference to the animator to initialise all the SMBs.
    private bool m_JumpDown;            // For the JumpDown property.
    private bool m_ResetJumpDown;       // Used to reset the JumpDown property if a frame has passed.


    void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }


    private void OnEnable()
    {
        // Find all the SMBs on the animator that inherit from CustomSMB.
        CustomSMB[] allSMBs = m_Animator.GetBehaviours <CustomSMB> ();
        for(int i = 0; i < allSMBs.Length; i++)
        {
            // For each SMB set it's userInput reference to this instance and run the initialisation function.
            allSMBs[i].userInput = this;
            allSMBs[i].Init (m_Animator);
        }
    }

    private void Update()
    {
        // If fly through isn't on so we can use input, get the input from the appropriate axes.
        if (InputEnabled)
        {
            Horizontal = Input.GetAxis("Horizontal");
            Vertical = Input.GetAxis("Vertical");
            SlowMo = Input.GetAxis("Fire1");
        }
        else
        {
            // Otherwise reset all the input.
            Horizontal = 0;
            Vertical = 0;
            SlowMo = 0;
        }

        // If there is some input, not the time.
        if (Horizontal != 0 || Vertical != 0 || SlowMo != 0)
            LastInputAt = Time.time;

        // If a FixedUpdate has happened since JumpDown was set, then reset it.
        if (m_ResetJumpDown)
        {
            m_ResetJumpDown = false;
            JumpDown = false;
        }

        // If the fly through is off and the JumpButton is pressed record it and the time.
        if (InputEnabled && Input.GetButtonDown("Jump"))
        {
            JumpDown = true;
            LastInputAt = Time.time;
        }
    }


    private void FixedUpdate ()
    {
        // Whenever a FixedUpdate happens, reset the JumpDown property.
        m_ResetJumpDown = true;
    }
}

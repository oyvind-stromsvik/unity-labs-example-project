using UnityEngine;

public class JumpingSMB : CustomSMB
{
    public float verticalJumpSpeed = 4f;        // The vertical value of speed when jumping is set.
    public float sprintSpeed = 4.2912f;         // The speed the character would be going if sprinting.
    public float jogSpeed = 2.822f;             // The speed the character would be going if jogging.
    public float walkSpeed = 1.19f;             // The speed the character would be going if walking.


    private Rigidbody m_Rigidbody;              // Reference to the rigidbody component.
    private Transform m_Transform;              // Reference to the transform component.


    public override void Init(Animator anim)
    {
        // Setting up the component references.
        m_Rigidbody = anim.GetComponent<Rigidbody>();
        m_Transform = anim.transform;
    }


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Find the intended character velocity based on user input.
        Vector3 intendedVelocityWeight = new Vector3(userInput.Horizontal, 0f, userInput.Vertical);

        // The intended speed based on user input is the velocity's magnitude.
        float intendedSpeedWeight = intendedVelocityWeight.magnitude;

        // The weighting cannot be greater that 1 (running diagonally isn't faster) so if it is, cap it's value at 1.
        intendedSpeedWeight = intendedSpeedWeight < 1f ? intendedSpeedWeight : 1f;

        // Create a variable to store the actual speed the character would be going.
        float speed = 0f;

        // If the intended speed is between stationary (zero input) and walking (0.5 input)...
        if (intendedSpeedWeight >= 0f && intendedSpeedWeight < 0.5f)
        {
            // Set the speed to an interpolation of no speed and the walk speed based on this weight.
            speed = Mathf.Lerp (0f, walkSpeed, intendedSpeedWeight * 2f);
        }
        // If the intended speed is between walking (0.5 input) and jogging (1 input)...
        else if (intendedSpeedWeight >= 0.5f && intendedSpeedWeight <= 1f)
        {
            // Set the speed to an interpolation of the walking speed and the jogging speed based on this weight.
            speed = Mathf.Lerp (walkSpeed, jogSpeed, (intendedSpeedWeight - 0.5f) * 2f);
        }
        
        // Create a velocity which hsa the intended velocity relative to the character and a vertical component of the jump speed.
        Vector3 velocity = m_Transform.TransformDirection (intendedVelocityWeight.normalized * speed) + Vector3.up * verticalJumpSpeed;

        // Set the rigidbody's velocity to this speed.
        m_Rigidbody.velocity = velocity;
    }
}

using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public float LastInputAt { get; private set; }      // The time when camera input was last detected.

    public float tiltMin = 45f;                         // The amount the camera can tilt down.
    public float tiltMax = 45f;                         // The amount the camera can tilt up.
    public float turnSpeed = 2f;                        // How fast the rotation happens.
    public float horizontalSmoothing = 0.1f;            // How smooth the horisontal rotation is.


    private float m_LookAngle;                          // For storing the y euler angle.
    private float m_TiltAngle;                          // For storing the x euler angle.
    private Transform m_Pivot;                          // Reference to the pivot gameobject.
    private float m_YVelocity;                          // Reference velocity for smoothing horizontal rotation.


    private void Awake ()
    {
        // Get the pivot reference.
        m_Pivot = transform.GetChild (0);
    }


    public void ResetAngles ()
    {
        // Reset the camera's rotation.
        m_LookAngle = 0;
        m_TiltAngle = 0;
        Rotate (0, 0);
    }


    private void LateUpdate ()
    {
        // Get the x and y input for rotation based on platform and mouse presence.
        float x = 0;
        float y = 0;

#if (UNITY_XBOXONE || UNITY_PS4) && !UNITY_EDITOR
		x = Input.GetAxis("Camera X");
		y = Input.GetAxis("Camera Y");
#else
        if (Input.mousePresent)
        {
            x = Input.GetAxis ("Mouse X");
            y = Input.GetAxis ("Mouse Y");
        }

        if (!Input.mousePresent || Mathf.Abs (x) < float.Epsilon)
            x = Input.GetAxis ("Camera X");
        if (!Input.mousePresent || Mathf.Abs (y) < float.Epsilon)
            y = Input.GetAxis ("Camera Y");
#endif

        Rotate (x, y);
    }


    private void Rotate (float x, float y)
    {
        // If there is input, store the time.
        if (x != 0 || y != 0)
            LastInputAt = Time.time;

        // Adjust the look angle by an amount proportional to the turn speed and horizontal input.
        m_LookAngle += x * turnSpeed;

        float eulerY = Mathf.SmoothDampAngle (transform.localEulerAngles.y, m_LookAngle, ref m_YVelocity,
            horizontalSmoothing);

        // Rotate the rig (the root object) around Y axis only:
        transform.localRotation = Quaternion.Euler (0f, eulerY, 0f);

        m_TiltAngle -= y * turnSpeed;
        // and make sure the new value is within the tilt range
        m_TiltAngle = Mathf.Clamp (m_TiltAngle, -tiltMin, tiltMax);

        // Tilt input around X is applied to the pivot (the child of this object)
        m_Pivot.localRotation = Quaternion.Euler (m_TiltAngle, 0f, 0f);
    }
}
using UnityEngine;
using UnityEngine.UI;

public class HealthMonitor : MonoBehaviour
{
    public float recoveryRate = 1f;     // The amount of health recovered per second.
    [HideInInspector]
    public float healthF;               // The health of the player as a float.


    private Animator m_Anim;            // Reference  to the animator component.
    private Text m_Text;                // Reference to the text component.
    private Slider m_Slider;            // Reference to the slider component.

    private readonly int m_HashChargingPara = Animator.StringToHash ("charging");       // For referencing the charging animator parameter.


    private int Health                  // The health of the player as an int.
    {
        get { return (int)healthF; }
        set { healthF = value; }
    }


    void Awake ()
    {
        // Get the references.
		m_Anim = transform.parent.GetComponent<Animator> ();
        m_Text = GetComponentInChildren<Text> ();
        m_Slider = GetComponentInChildren<Slider> ();
    }


    void Start ()
    {
        // Reset the health.
        Health = 100;
    }


    void Update ()
    {
        // If the player has lost health the animator is charging and health should be recovered.
        if (healthF < 100f)
        {
			healthF += recoveryRate * Time.deltaTime;
			m_Anim.SetBool(m_HashChargingPara, true);
		}
        else 
		{
            // Otherwise reset the health and the animator is no longer charging.
			healthF = 100f;
			m_Anim.SetBool(m_HashChargingPara, false);
		}

        // Set the text and slider based on the health.
        m_Text.text = Health.ToString ();
        m_Slider.value = healthF;
    }
}

using System;
using UnityEngine;

public class DoorEventSMB : CustomSMB
{
	public enum AudioType
	{
		OpenDoor,
		CloseDoor
	}


	public AudioType audioType;                 // Whether the audio is for opening or closing the door.
    public bool offsetStartPosition = true;     // For offsetting the audio by the same amount that OnStateEnter is late.


    private DoorManager m_Door;
    private AudioSource m_Audio;


    public override void Init (Animator anim)
    {
        m_Door = anim.GetComponent<DoorManager> ();
        m_Audio = anim.GetComponent<AudioSource> ();
    }


    public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
        // Exit if there's no door manager reference.
        if (!m_Door) return;

        // Call any event subscribers appropriate for openning or closing
		switch (audioType)
        {
		case AudioType.OpenDoor:
            m_Door.OnBeginOpening.Invoke();
			break;
		case AudioType.CloseDoor:
            m_Door.OnBeginClosing.Invoke();
			break;
		}

        // Get an appropriate audio clip for openning or closing.
        var clip = GetClipFromDoor(m_Door, audioType);
		if (!clip) return;

        // Play the clip.
        m_Audio.clip = clip;
        m_Audio.Play();

        // Offset the audio by an amount equal to the length of time since the state started.
		if (offsetStartPosition)
            m_Audio.time = Mathf.Clamp(stateInfo.length * stateInfo.normalizedTime, 0f, clip.length);
	}


	private static AudioClip GetClipFromDoor(DoorManager door, AudioType type)
	{
        // Return the appropriate audio clip for openning or closing.
		switch(type)
		{
		case AudioType.OpenDoor:
			return door.AudioOpenDoorClip;
		case AudioType.CloseDoor:
			return door.AudioCloseDoorClip;
		default:
			throw new ArgumentException ("Unknown audio type " + type);
		}
	}
}

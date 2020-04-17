using UnityEngine;
using System.Collections;

public class VisibilityProxy : MonoBehaviour
{

    private int m_LastVisibleAtFrame;

    public void OnWillRenderObject()
    {
        m_LastVisibleAtFrame = Time.frameCount;
    }

    public bool WasVisibleThisFrame()
    {
        return m_LastVisibleAtFrame == Time.frameCount;
    }
}

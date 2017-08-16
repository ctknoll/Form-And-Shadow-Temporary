using UnityEngine;

public class MovingPlatformTriggerZone : MonoBehaviour
{
    public bool m_PlayerChilded;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            m_PlayerChilded = true;
            other.gameObject.transform.parent = gameObject.transform;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            m_PlayerChilded = false;
            other.gameObject.transform.parent = null;
        }
    }
}

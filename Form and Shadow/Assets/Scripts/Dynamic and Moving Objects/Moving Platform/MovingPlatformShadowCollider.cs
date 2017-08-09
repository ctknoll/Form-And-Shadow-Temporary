using UnityEngine;

public class MovingPlatformShadowCollider : MonoBehaviour
{
    [HideInInspector] public bool m_PlayerChildedIn2D;

    void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			m_PlayerChildedIn2D = true;
			other.gameObject.transform.parent = gameObject.transform;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
		    m_PlayerChildedIn2D = false;
			other.gameObject.transform.parent = null;
		}
	}
}
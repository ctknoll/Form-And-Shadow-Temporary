using UnityEngine;

public class MovingPlatformShadowCollider : MonoBehaviour {
    [HideInInspector]
    public bool playerChildedIn2D;
    void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			playerChildedIn2D = true;
			other.gameObject.transform.parent = gameObject.transform;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
		    playerChildedIn2D = false;
			other.gameObject.transform.parent = null;
		}
	}
}
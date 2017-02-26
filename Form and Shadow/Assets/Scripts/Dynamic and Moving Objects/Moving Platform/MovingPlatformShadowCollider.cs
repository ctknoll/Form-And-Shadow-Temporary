using UnityEngine;

public class MovingPlatformShadowCollider : MonoBehaviour {
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			if(GetComponentInParent<PulleySystemPlatform>())
				GetComponentInParent<PulleySystemPlatform>().playerChildedIn2D = true;
			other.gameObject.transform.parent = gameObject.transform;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			if(GetComponentInParent<PulleySystemPlatform>())
				GetComponentInParent<PulleySystemPlatform>().playerChildedIn2D = false;
			other.gameObject.transform.parent = null;
		}
	}
}
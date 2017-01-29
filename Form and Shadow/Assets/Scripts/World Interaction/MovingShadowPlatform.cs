using System.Collections;
using UnityEngine;

public class MovingShadowPlatform : MonoBehaviour {
	void OnTriggerStay(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			other.gameObject.transform.parent = gameObject.transform;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			other.gameObject.transform.parent = null;
		}
	}
}
using UnityEngine;

public class PulleySystemEnd : MonoBehaviour {
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Move Platform")
			other.gameObject.transform.parent.GetComponent<PulleySystemPlatform>().atEndOfRoute = true;
	}
}

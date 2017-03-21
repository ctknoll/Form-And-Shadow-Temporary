using UnityEngine;

public class TorchObjectControl : MonoBehaviour {

	void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Shadow Wall")
        {

        }
    }
	
	void OnTriggerExit (Collider other)
    {
		if(other.gameObject.tag == "Shadow Wall")
        {

        }
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownwardShadowControl : MonoBehaviour {
	
	void Update ()
    {
        if (PlayerMovement.shiftingOut || PlayerMovement.in3DSpace)
        {
            gameObject.GetComponent<Light>().enabled = true;
        }
        else
        {
            gameObject.GetComponent<Light>().enabled = false;
        }
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownwardShadowControl : MonoBehaviour {
	
	void Update ()
    {
        if (!PlayerMovement.in3DSpace)
        {
            gameObject.GetComponent<Light>().enabled = false;
        }
        else
        {
            gameObject.GetComponent<Light>().enabled = true;
        }
	}
}

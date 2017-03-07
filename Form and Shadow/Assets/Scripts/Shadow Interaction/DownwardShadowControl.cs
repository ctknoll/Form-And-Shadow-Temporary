using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

-- Downward Shadow Control --
Attached to a downward facing light in the scene, lighting the level.
It's active when the player is not in 2D.

*/

public class DownwardShadowControl : MonoBehaviour 
{
	void Update ()
    {
		if (PlayerMovement.shadowShiftingIn || PlayerMovement.shadowShiftingOut || PlayerMovement.in3DSpace)
        {
            gameObject.GetComponent<Light>().enabled = true;
        }
        else
        {
            gameObject.GetComponent<Light>().enabled = false;
        }
	}
}

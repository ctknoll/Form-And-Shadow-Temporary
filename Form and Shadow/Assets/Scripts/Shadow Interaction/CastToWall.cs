using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastToWall
{
	public static bool castToWall(GameObject player)
    {
        RaycastHit hitInfo;
        //checks if theres nothing between the player and the camera
        //should cast a ray from the player away from the camera, and return true if there is a plane within 50 units
		if (Physics.Raycast(player.transform.position, LightSourceControl.lightSourceDirection, out hitInfo, 50))
        {
			player.GetComponent<PlayerMovement>().shiftedPlane = hitInfo.collider.gameObject;
            //this doesn't check for planes; unity primitives are arbitrary meshes. We should label our meldable walls somehow!
            //remove this object from the camera
            player.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            player.layer = 9;
            return true;
         }
		return false;
	}
			
    //this is real hacky shit, and actually wont work for what we want tbh
    public static bool removeFromWall(GameObject player)
    {
        player.GetComponent<PlayerMovement>().shiftedPlane = null;
        player.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        player.layer = 0;
        return true;
    }
}

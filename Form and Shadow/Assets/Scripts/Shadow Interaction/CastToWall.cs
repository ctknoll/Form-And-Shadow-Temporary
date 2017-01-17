using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastToWall
{
    public static bool castToWall(GameObject player)
    {
        Vector3 dir = player.transform.position - Camera.main.transform.position;
        RaycastHit hitInfo;
        Debug.Log(Physics.Linecast(Camera.main.transform.position, dir, out hitInfo));
        Debug.Log(Physics.Raycast(player.transform.position, -1 * (Camera.main.transform.position - player.transform.position), out hitInfo, 50));
        //checks if theres nothing between the player and the camera
        if (Physics.Linecast(Camera.main.transform.position, player.transform.position, out hitInfo))
        {
            //should cast a ray from the player away from the camera, and return true if there is a plane within 50 units
            if (Physics.Raycast(player.transform.position, -1 * (Camera.main.transform.position - player.transform.position), out hitInfo, 50))
            {
                player.GetComponent<PlayerMovement>().shiftedPlane = hitInfo.collider.gameObject;
                Debug.Log(Physics.Raycast(player.transform.position, -1 * (Camera.main.transform.position - player.transform.position), out hitInfo, 50));
                //this doesn't check for planes; unity primitives are arbitrary meshes. We should label our meldable walls somehow!
                //remove this object from the camera
                player.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                player.layer = 9;
                return true;
            }
            return false;
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

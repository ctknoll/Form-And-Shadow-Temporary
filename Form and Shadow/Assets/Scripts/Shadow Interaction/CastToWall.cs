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
            player.GetComponent<CharacterController>().Move((hitInfo.point - player.transform.position) * (hitInfo.distance + 5));
            Sprite twoDimSprite = Resources.Load<Sprite>("sprite");
            GameObject twoDim = new GameObject();
            twoDim.transform.position = hitInfo.point + hitInfo.normal;
            twoDim.transform.rotation = Quaternion.LookRotation(hitInfo.collider.gameObject.transform.up);
            twoDim.AddComponent<SpriteRenderer>();
            twoDim.GetComponent<SpriteRenderer>().sprite = twoDimSprite;
            twoDim.AddComponent<CharacterController>();
            twoDim.AddComponent<PlayerMovement>();
            twoDim.GetComponent<PlayerMovement>().shiftedPlane = hitInfo.collider.gameObject;
            twoDim.GetComponent<PlayerMovement>().movementSpeed = player.GetComponent<PlayerMovement>().movementSpeed;
            GameObject.Find("Main Camera").GetComponent<CameraControl>().target = twoDim.transform;
            GameObject.Find("Main Camera").GetComponent<CameraControl>().normalToWall = hitInfo.normal;
            player.SetActive(false);
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

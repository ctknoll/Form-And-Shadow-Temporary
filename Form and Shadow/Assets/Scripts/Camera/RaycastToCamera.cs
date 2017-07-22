using UnityEngine;

/*
    --RaycastToCamera--
    Handles the logic of an object attached to the Player_Character
    prefab called Player_Camera_Raycast always looks at the camera
    object and casts a ray in its direction, and changing the distance
    the camera should be to the player if the raycast collides with an 
    object on the way, allowing the camera to dynamically move in and 
    out of the player when navigating complex geometry.

*/

public class RaycastToCamera : MonoBehaviour {
    public static float distance;
    Transform target;
    void Start()
    {
        target = Camera.main.transform;
        distance = target.GetComponent<NewCameraControl>().m_DistanceToPlayer3D;
    }
	void Update ()
	{
		transform.LookAt(target);

		RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit))
        {
            if (hit.collider.gameObject.tag != "Player")
                distance = hit.distance - 0.5f;
            else
                distance = target.GetComponent<NewCameraControl>().m_DistanceToPlayer3D;
        }
	}
}

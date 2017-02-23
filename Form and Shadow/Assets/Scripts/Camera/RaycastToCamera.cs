using UnityEngine;

public class RaycastToCamera : MonoBehaviour {
	public Transform target;

    public static float distance;

    void Start()
    {
        distance = target.GetComponent<CameraControl>().distanceToPlayer3D;
    }
	void Update ()
	{
		transform.LookAt(target);

		RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, ~ 1 << 15))
        {
            if(hit.collider.gameObject.tag != "Player")
                distance = hit.distance - 0.5f;
        }
        else
            distance = target.GetComponent<CameraControl>().distanceToPlayer3D;
	}
}

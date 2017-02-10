using UnityEngine;

public class MovementReference : MonoBehaviour {
	public GameObject camera;

	void Update () 
	{
		transform.eulerAngles = new Vector3(0, camera.transform.eulerAngles.y, 0);
	}
}

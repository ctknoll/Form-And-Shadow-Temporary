using UnityEngine;

public class MovementReference : MonoBehaviour {
	public GameObject mainCamera;

	void Update () 
	{
		transform.eulerAngles = new Vector3(0, mainCamera.transform.eulerAngles.y, 0);
	}
}

using UnityEngine;

public class MovementReference : MonoBehaviour {
	private GameObject mainCamera;
    void Start()
    {
        mainCamera = GetComponentInParent<PlayerMovement>().mainCamera;
    }
	void Update () 
	{
		transform.eulerAngles = new Vector3(0, mainCamera.transform.eulerAngles.y, 0);
	}
}

using UnityEngine;

public class PressureSwitch : MonoBehaviour 
{
	public bool canInteract;
	public bool pressed;
	private float activationTime = 0;
	private Vector3 startPos;

	private float holdCubeX;
	private float holdCubeZ;

	public void Start()
	{
		startPos = transform.position;
		holdCubeX = 0;
		holdCubeZ = 0;
	}

	// Update is called once per frame
	public void Update () 
	{
		if (pressed && activationTime < 30) 
		{
			transform.position += new Vector3(0, -(transform.parent.lossyScale.y / 60), 0);
			activationTime++;
		}

		if (!pressed && activationTime > 0) 
		{
			transform.position += new Vector3(0, (transform.parent.lossyScale.y / 60), 0);
			activationTime--;
		}
			
	}

	public void OnTriggerStay(Collider other)
	{
        if ((other.gameObject.tag == "Player" && !PlayerMovement.shadowMelded && !PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut) || other.gameObject.tag == "Push Cube")
            pressed = true;
		if (other.gameObject.tag == "Push Cube") 
		{
			other.transform.parent = gameObject.transform;
			if (holdCubeX == 0)
				holdCubeX = other.transform.position.x;
			if (holdCubeZ == 0)
				holdCubeZ = other.transform.position.z;
			other.transform.position = new Vector3(holdCubeX, other.transform.position.y, holdCubeZ);
		}
    }

	public void OnTriggerExit(Collider other)
    {
        if ((other.gameObject.tag == "Player" && !PlayerMovement.shadowMelded && !PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut) || other.gameObject.tag == "Push Cube")
            pressed = false;
	}
		
}

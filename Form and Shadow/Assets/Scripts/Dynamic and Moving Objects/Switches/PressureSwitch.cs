using UnityEngine;

public class PressureSwitch : MonoBehaviour 
{
	public bool canInteract;
	public bool pressed;
	private float activationTime = 0;

	private float holdCubeX;
	private float holdCubeZ;
	
	public void Start()
	{
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
        if(PlayerMovement.shadowShiftingIn || PlayerMovement.shadowShiftingOut)
        {
            pressed = false;
        }
	}

    public void OnTriggerStay(Collider other)
	{
        if ((other.gameObject.tag == "Player" && !PlayerMovement.shadowMelded && !PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut) || other.gameObject.tag == "Push Cube")
            pressed = true;
		//Holds Push Cube in place so player cannot move it after activating pressure plate
		if (other.gameObject.tag == "Push Cube") //Removed by DC by accident. Re-added from backup by JSM 2.27.17
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

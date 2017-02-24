using UnityEngine;

public class PressureSwitch : MonoBehaviour 
{
	public bool canInteract;
	public bool pressed;
	private float activationTime = 0;
	private Vector3 startPos;


	public void Start()
	{
		startPos = transform.position;
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

	public void OnTriggerEnter(Collider other)
	{
        if(other.gameObject.tag == "Player" || other.gameObject.tag == "Push Cube")
            pressed = true;
	}

	public void OnTriggerStay(Collider other)
	{
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Push Cube")
            pressed = true;
	}

	public void OnTriggerExit(Collider other)
	{
		pressed = false;
	}
		
}

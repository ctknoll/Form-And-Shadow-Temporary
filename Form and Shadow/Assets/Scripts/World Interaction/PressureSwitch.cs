using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureSwitch : MonoBehaviour 
{

	public bool canInteract;
	public bool active;
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
		active = (activationTime > 0);
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5);
		RaycastHit hit;
		foreach(Collider coll in hitColliders)
		{
			if (Physics.Raycast (new Vector3 (coll.transform.position.x, coll.transform.position.y - (coll.transform.lossyScale.y), coll.transform.position.z), -Vector3.up, out hit, .1f))
			{
					if (hit.collider == GetComponent<Collider>()) 
					{
						pressed = true;
					} 
					else
						pressed = false;
			}
			else 
				pressed = false;

		}
		
		if (pressed && activationTime < 30) 
		{
			transform.position += new Vector3(0, -(transform.lossyScale.y / 60), 0);
			activationTime++;
		}
		else if (!pressed && activationTime > 0) 
		{
			transform.position += new Vector3(0, (transform.lossyScale.y / 60), 0);
			activationTime--;
		}
			
	}
		
}

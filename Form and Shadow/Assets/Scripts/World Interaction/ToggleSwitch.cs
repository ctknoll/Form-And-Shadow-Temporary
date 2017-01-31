using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleSwitch : MonoBehaviour {

	public bool canInteract;
	public bool active;
	public float activationTime = -1;
	private float runningTime = -1;


	public void Start()
	{
	}

	// Update is called once per frame
	public void Update () 
	{
		active = (runningTime != 0);
		Vector3 distance = GameObject.Find ("Player_Character").transform.position - transform.position;
		if (distance.magnitude <= 5) 
		{
			if (Input.GetButtonDown ("Grab")) 
			{
				if(active) 
				{
					active = false;
					runningTime = -1;
				} 
				else 
				{
					active = true;
					runningTime = activationTime;
				}

			}

		}

		if (runningTime > 0) 
		{
			runningTime--;
		} 
		else if (runningTime == 0) 
		{
			active = false;
		}
	}
}

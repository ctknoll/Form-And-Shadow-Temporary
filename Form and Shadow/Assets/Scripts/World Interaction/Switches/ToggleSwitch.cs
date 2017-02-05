using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleSwitch : MonoBehaviour {

	public bool canInteract;
	public bool active = false;
	public float activationTime;
	private float runningTime = 0;


	public void Start()
	{
	}

	// Update is called once per frame
	public void Update () 
	{
        Debug.Log(active);
        if (active)
            Debug.Log(runningTime);
		active = (runningTime != 0);
		Vector3 distance = GameObject.Find ("Player_Character").transform.position - transform.position;
		if (distance.magnitude <= 2) 
		{
			if (Input.GetButtonDown ("Grab")) 
			{
				if(active) 
				{
					active = false;
					runningTime = 0;
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
			runningTime -= Time.deltaTime;
		} 
		else if (runningTime <= 0 && runningTime > -1) 
		{
			active = false;
            runningTime = 0;
		}
	}
}

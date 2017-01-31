using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformPressureSwitch : PressureSwitch {

	public GameObject movingBlock;
	private float tempMoveSpeed;
	private bool locked;

	// Use this for initialization
	void Start () 
	{
		base.Start();
	}
	
	// Update is called once per frame
	public void Update () 
	{
		base.Update();
		if (active) 
		{
			if (movingBlock.GetComponent<MovingPlatform> () != null) 
			{
				movingBlock.GetComponent<MovingPlatform>().stopTime = true;
				locked = true;
			}
		} 
		else 
		{
			if (movingBlock.GetComponent<MovingPlatform>() != null) 
			{
				if (locked) 
				{

					movingBlock.GetComponent<MovingPlatform>().stopTime = false;
					locked = false;
				}
					
			}
		}
	}
}

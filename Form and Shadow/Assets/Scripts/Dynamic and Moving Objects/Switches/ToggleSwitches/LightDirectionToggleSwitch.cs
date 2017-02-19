using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDirectionToggleSwitch : ToggleSwitch
{

    public GameObject[] lightSources;
    public float rotateDegrees;
    private bool locked;
	public bool turnClockwise;

    // Use this for initialization
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public void Update()
    {
        if (pressed)
        {
            if (!locked)
            {
				foreach (GameObject lightSource in lightSources) 
				{
					if (lightSource.GetComponent<LightSourceControl>() != null)
					{
						lightSource.GetComponent<LightSourceControl>().turnLightSource(turnClockwise);
						locked = true;
					}
				}
					
            }
        }
        else
        {
			foreach (GameObject lightSource in lightSources) 
			{
				if (lightSource.GetComponent<LightSourceControl>() != null)
				{
					locked = false;
				}
			}
        }
        base.Update();
    }
}
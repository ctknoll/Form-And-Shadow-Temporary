using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDirectionToggleSwitch : ToggleSwitch
{

    public GameObject lightSource;
    public float rotateDegrees;
    private bool locked;

    // Use this for initialization
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public void Update()
    {
        if (active)
        {
            if (!locked)
            {
                if (lightSource.GetComponent<LightSourceControl>() != null)
                {
                    lightSource.GetComponent<LightSourceControl>().turnLightSource();
                    locked = true;
                }
            }
        }
        else
        {
            if (lightSource.GetComponent<LightSourceControl>() != null)
            {
                locked = false;
            }
        }
        base.Update();
    }
}
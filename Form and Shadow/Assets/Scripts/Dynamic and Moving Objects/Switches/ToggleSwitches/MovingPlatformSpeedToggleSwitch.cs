using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformSpeedToggleSwitch : ToggleSwitch
{

    public GameObject movingPlatform;
    private float tempMoveSpeed;
    public float percentSlow;
    public bool haltWhenNotPressed;
    private bool locked;

    // Use this for initialization
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public void Update()
    {
        if (haltWhenNotPressed) movingPlatform.GetComponent<MovingPlatform>().slowValue = 0;
        if (pressed)
        {
            if (movingPlatform.GetComponent<MovingPlatform>() != null)
            {
                movingPlatform.GetComponent<MovingPlatform>().slowValue = 1 - (percentSlow / 100);
                locked = true;
            }
        }
       else
        {
            if (movingPlatform.GetComponent<MovingPlatform>() != null)
            {
                if (locked)
                {
                    movingPlatform.GetComponent<MovingPlatform>().slowValue = 1;
                    locked = false;
                }

            }
        }
        base.Update();
    }
}

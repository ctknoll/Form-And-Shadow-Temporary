using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformSpeedToggleSwitch : ToggleSwitch
{

    public GameObject movingBlock;
    private float tempMoveSpeed;
    public float percentSlow;
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
            if (movingBlock.GetComponent<MovingPlatform>() != null)
            {
                movingBlock.GetComponent<MovingPlatform>().slowValue = 1 - (percentSlow / 100);
                locked = true;
            }
        }
       else
        {
            if (movingBlock.GetComponent<MovingPlatform>() != null)
            {
                if (locked)
                {
                    movingBlock.GetComponent<MovingPlatform>().slowValue = 1;
                    locked = false;
                }

            }
        }
        base.Update();
    }
}

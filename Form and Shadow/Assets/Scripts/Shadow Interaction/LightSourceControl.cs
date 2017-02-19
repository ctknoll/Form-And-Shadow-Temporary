using UnityEngine;

public class LightSourceControl : MonoBehaviour 
{
	public Vector3 lightSourceDirection;

	public bool zAxisMovement;
	public bool xAxisMovement;

	void Start ()
	{
        lightSourceDirection = transform.forward;
		CheckLightingDirection();
	}

	void Update () 
	{
        //transform.Rotate(0, 90, 0);
        lightSourceDirection = transform.forward;

    }

	public void CheckLightingDirection()
	{
        if (lightSourceDirection == GameObject.Find("Light Reference").transform.forward || 
			-1 * lightSourceDirection == GameObject.Find("Light Reference").transform.forward) 
		{
            zAxisMovement = true;
			xAxisMovement = false;
        }
		else if(lightSourceDirection == GameObject.Find("Light Reference").transform.right || 
			-1 * lightSourceDirection == GameObject.Find("Light Reference").transform.right) 
		{
            zAxisMovement = false;
			xAxisMovement = true;
		}
    }

    //This function is suboptimal -- it casts EVERY time, creating a lot of unnecesary overhead
	public void turnLightSource(bool turnCounterClockwise)
    {
        
		float clockWiseVal = (turnCounterClockwise ? -1 : 1);
		transform.Rotate(0, clockWiseVal * 90, 0);
        lightSourceDirection = transform.forward;
        CheckLightingDirection();
    }
}

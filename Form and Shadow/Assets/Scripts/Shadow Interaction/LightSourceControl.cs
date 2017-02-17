using UnityEngine;

public class LightSourceControl : MonoBehaviour 
{
	public static Vector3 lightSourceDirection;

	public static bool zAxisMovement;
	public static bool xAxisMovement;

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
    public void turnLightSource()
    {
        GameObject.Find("Master_Directional_Light").transform.Rotate(0, -90, 0);
        lightSourceDirection = GameObject.Find("Master_Directional_Light").transform.forward;
        CheckLightingDirection();
    }
}

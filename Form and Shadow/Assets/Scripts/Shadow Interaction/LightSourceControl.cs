using UnityEngine;

public class LightSourceControl : MonoBehaviour 
{
	public Quaternion lightSourceStartRotation;
	public Vector3 lightSourceDirection;

	public bool zAxisMovement;
	public bool xAxisMovement;

	void Start ()
	{
		lightSourceStartRotation = transform.rotation;
		lightSourceDirection = transform.forward;
		CheckLightingDirection();
	}

	void Update () 
	{
        lightSourceDirection = transform.forward;

    }

	public void CheckLightingDirection()
	{
        if (lightSourceDirection == GameObject.Find("Lighting_Reference").transform.forward || 
			-1 * lightSourceDirection == GameObject.Find("Lighting_Reference").transform.forward) 
		{
            zAxisMovement = true;
			xAxisMovement = false;
        }
		else if(lightSourceDirection == GameObject.Find("Lighting_Reference").transform.right || 
			-1 * lightSourceDirection == GameObject.Find("Lighting_Reference").transform.right) 
		{
            zAxisMovement = false;
			xAxisMovement = true;
		}
    }

    //This function is suboptimal -- it casts EVERY time, creating a lot of unnecesary overhead
	public void turnLightSource(bool turnClockwise)
    {
        
		float clockWiseVal = (turnClockwise ? -1 : 1);
		transform.Rotate(0, clockWiseVal * 90, 0);
        lightSourceDirection = transform.forward;
        CheckLightingDirection();
    }
}

using UnityEngine;

/*

-- Light Source Control --
Attached to each light source. Controls light direction and 
sets axis-control for shadow casting.

*/

public class LightSourceControl : MonoBehaviour 
{

	//The starting and current directions of the light
	public Quaternion lightSourceStartRotation;
	public Vector3 lightSourceDirection;

	//The variables defining whether a light is shining along the
	//X axis or the Z axis. Useful for casting shadows
	public bool zAxisMovement;
	public bool xAxisMovement;

	//Stores a light's default position, as well as checks a 
	//light's direction
	void Start ()
    {
		lightSourceStartRotation = transform.rotation;
		lightSourceDirection = transform.forward;
		CheckLightingDirection();
	}

	//Checks the light's direction relative to the master lighting reference (true forward)
	public void CheckLightingDirection()
	{
        if (lightSourceDirection == GameObject.Find("Lighting").transform.forward || 
			lightSourceDirection == -GameObject.Find("Lighting").transform.forward) 
		{
            zAxisMovement = true;
			xAxisMovement = false;
        }
		else if(lightSourceDirection == GameObject.Find("Lighting").transform.right || 
			lightSourceDirection == -GameObject.Find("Lighting").transform.right) 
		{
            zAxisMovement = false;
			xAxisMovement = true;
		}
    }

    //This function is suboptimal -- it casts EVERY time, creating a lot of unnecesary overhead
	public void TurnLightSource(bool turnClockwise)
    {
        
		float clockWiseVal = (turnClockwise ? -1 : 1);
		transform.Rotate(0, clockWiseVal * 90, 0);
        lightSourceDirection = transform.forward;
        CheckLightingDirection();
    }
}

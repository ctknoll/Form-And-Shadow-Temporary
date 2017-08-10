using UnityEngine;
using System.Collections.Generic;
using System.Collections;
/*

-- Light Source Control --
Attached to each light source. Controls light direction and 
sets axis-control for shadow casting.

*/

public class LightSourceControl : MonoBehaviour
{
    [HideInInspector] public Vector3 m_LightSourceForward;

	//The variables defining whether a light is shining along the
	//X axis or the Z axis. Useful for casting shadows
    public enum FacingDirection { North, East, South, West };
    public FacingDirection m_CurrentFacingDirection;

    GameObject m_LightingMasterControl;

	//Stores a light's default position, as well as checks a 
	//light's direction
	void Awake ()
    {
        m_LightSourceForward = transform.forward;
        m_LightingMasterControl = GameObject.Find("Lighting_Master_Control");
		CheckLightingDirection();
	}

	//Checks the light's direction relative to the master lighting reference (true forward)
	public void CheckLightingDirection()
	{
        m_LightSourceForward = transform.forward;
        if (m_LightSourceForward == m_LightingMasterControl.transform.forward)
            m_CurrentFacingDirection = FacingDirection.North;
        else if (m_LightSourceForward == -m_LightingMasterControl.transform.forward)
            m_CurrentFacingDirection = FacingDirection.South;
        else if (m_LightSourceForward == m_LightingMasterControl.transform.right)
            m_CurrentFacingDirection = FacingDirection.East;
        else if (m_LightSourceForward == -m_LightingMasterControl.transform.right)
            m_CurrentFacingDirection = FacingDirection.West;
    }

    //This function is suboptimal -- it casts EVERY time, creating a lot of unnecesary overhead
	public void TurnLightSource(bool turnClockwise)
    {
		float clockWiseVal = (turnClockwise ? -1 : 1);
        transform.eulerAngles += new Vector3(0, clockWiseVal * 90, 0);
        m_LightSourceForward = transform.forward;
        CheckLightingDirection();
    }
}

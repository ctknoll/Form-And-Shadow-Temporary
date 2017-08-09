using UnityEngine;

public class PressureSwitchSlowPlatform : PressureSwitch
{
    [SerializeField] float m_PlatformSlowValue = 0.5f;
    [SerializeField] GameObject [] m_TargetPlatforms;

	new void Start ()
    {
        base.Start();
	}
	
	new void Update ()
    {
        base.Update();
        if(m_Pressed)
        {
            foreach(GameObject tarPlatform in m_TargetPlatforms)
            {
                tarPlatform.GetComponentInChildren<MovingPlatform>().slowValue = m_PlatformSlowValue;
            }
        }
        else
        {
            foreach(GameObject tarPlatform in m_TargetPlatforms)
            {
                tarPlatform.GetComponentInChildren<MovingPlatform>().slowValue = 1f;
            }
        }
	}
}

using UnityEngine;

public class ToggleSwitchLightSource : ToggleSwitch
{
    [SerializeField] GameObject m_TargetLightSource;
    [SerializeField] bool m_TurnLightSourceClockwise;
    [SerializeField] GameObject m_LightCone;

    Animator lightConeAnimator;
    bool turnedLightFlippedOn;
    bool turnedLightFlippedOff;

	new void Start ()
    {
        base.Start();
        lightConeAnimator = m_LightCone.GetComponent<Animator>();
        turnedLightFlippedOn = false;
        turnedLightFlippedOff = true;
	}
	
	new void Update ()
    {
        base.Update();
        if(m_Pressed)
        {
            if(!turnedLightFlippedOn)
            {
                m_TargetLightSource.GetComponent<LightSourceControl>().TurnLightSource(m_TurnLightSourceClockwise);
                turnedLightFlippedOn = true;
                turnedLightFlippedOff = false;
            }
        }
        else
        {
            if(!turnedLightFlippedOff)
            {
                m_TargetLightSource.GetComponent<LightSourceControl>().TurnLightSource(m_TurnLightSourceClockwise);
                turnedLightFlippedOn = false;
                turnedLightFlippedOff = true;
            }
        }
        UpdateLightConeAnimator();
	}

    void UpdateLightConeAnimator()
    {
        switch(m_TargetLightSource.GetComponent<LightSourceControl>().m_CurrentFacingDirection)
        {
            case LightSourceControl.FacingDirection.North:
                lightConeAnimator.SetBool("FacingNorth", true);
                lightConeAnimator.SetBool("FacingEast", false);
                lightConeAnimator.SetBool("FacingSouth", false);
                lightConeAnimator.SetBool("FacingWest", false);
                break;
            case LightSourceControl.FacingDirection.East:
                lightConeAnimator.SetBool("FacingNorth", false);
                lightConeAnimator.SetBool("FacingEast", true);
                lightConeAnimator.SetBool("FacingSouth", false);
                lightConeAnimator.SetBool("FacingWest", false);
                break;
            case LightSourceControl.FacingDirection.South:
                lightConeAnimator.SetBool("FacingNorth", false);
                lightConeAnimator.SetBool("FacingEast", false);
                lightConeAnimator.SetBool("FacingSouth", true);
                lightConeAnimator.SetBool("FacingWest", false);
                break;
            case LightSourceControl.FacingDirection.West:
                lightConeAnimator.SetBool("FacingNorth", false);
                lightConeAnimator.SetBool("FacingEast", false);
                lightConeAnimator.SetBool("FacingSouth", false);
                lightConeAnimator.SetBool("FacingWest", true);
                break;
        }
    }
}

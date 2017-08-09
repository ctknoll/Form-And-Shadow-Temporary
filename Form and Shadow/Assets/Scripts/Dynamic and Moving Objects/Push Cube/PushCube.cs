using UnityEngine;

public class PushCube : MonoBehaviour
{
    [HideInInspector] public bool m_PlayerCanInteract;
    [HideInInspector] public bool m_Grabbed;

	void Update()
	{
        UpdateGrabbingInput();
        if (m_Grabbed)
            transform.parent = PlayerMotor.m_Instance.gameObject.transform;
        else
            transform.parent = null;
    }

    void UpdateGrabbingInput()
    {
        if(m_PlayerCanInteract)
        {
            if(!m_Grabbed)
            {
                if (Input.GetButtonDown("Grab"))
                {
                    Grab();
                }
            }
            if (Input.GetButtonUp("Grab"))
            {
                Release();
            }
        }
    }

    void Grab()
    {
        m_Grabbed = true;
        PlayerMotor.m_Instance.m_GrabbedObjectTransform = gameObject.transform;
        PlayerMotor.m_Instance.SnapAlignCharacterWithGrabbedObject();
        PlayerShadowInteraction.m_CurrentPlayerState = PlayerShadowInteraction.PLAYERSTATE.GRABBING;
    }

    public void Release()
    {
        m_Grabbed = false;
        PlayerMotor.m_Instance.m_GrabbedObjectTransform = null;
        PlayerShadowInteraction.m_CurrentPlayerState = PlayerShadowInteraction.PLAYERSTATE.FORM;
    }
}

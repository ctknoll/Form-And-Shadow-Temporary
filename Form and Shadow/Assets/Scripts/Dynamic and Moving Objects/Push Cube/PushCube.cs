using UnityEngine;

public class PushCube : MonoBehaviour {
    public bool canInteract;
    public bool grabbed;

	void Start()
	{
	}

	void Update()
	{
        UpdateGrabbingInput();

	}

    void UpdateGrabbingInput()
    {
        if(canInteract)
        {
            if(!grabbed)
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
        grabbed = true;
        PlayerMotor.m_Instance.m_GrabbedObjectTransform = gameObject.transform;
        PlayerShadowInteraction.m_CurrentPlayerState = PlayerShadowInteraction.PLAYERSTATE.GRABBING;
    }

    public void Release()
    {
        grabbed = false;
        PlayerMotor.m_Instance.m_GrabbedObjectTransform = null;
        PlayerShadowInteraction.m_CurrentPlayerState = PlayerShadowInteraction.PLAYERSTATE.FORM;
    }
}

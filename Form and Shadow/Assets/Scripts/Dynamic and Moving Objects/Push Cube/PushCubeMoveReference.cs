using UnityEngine;

public class PushCubeMoveReference : MonoBehaviour
{
	PushCube m_PushCube;

	void Start()
	{
		m_PushCube = GetComponentInParent<PushCube>();
	}

	void OnTriggerEnter(Collider other)
	{
        if (PlayerShadowInteraction.m_CurrentPlayerState != PlayerShadowInteraction.PLAYERSTATE.FORM || GameController.paused)
            return;

        if (other.gameObject.CompareTag("Player"))
		{
            m_PushCube.canInteract = true;
            PlayerMotor.m_Instance.m_GrabbedObjectPlayerSide = gameObject.transform;
        }
	}

	void OnTriggerExit(Collider other)
	{
        if (GameController.paused)
            return;

        if (other.gameObject.CompareTag("Player"))
        {
            m_PushCube.canInteract = false;
            PlayerMotor.m_Instance.m_GrabbedObjectPlayerSide = null;
            m_PushCube.Release();
        }
    }
}


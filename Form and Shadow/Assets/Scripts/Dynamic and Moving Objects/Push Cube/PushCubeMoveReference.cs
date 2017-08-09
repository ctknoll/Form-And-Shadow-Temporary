using UnityEngine;

public class PushCubeMoveReference : MonoBehaviour
{
	PushCube pushCube;

	void Start()
	{
		pushCube = GetComponentInParent<PushCube>();
	}

	void OnTriggerEnter(Collider other)
	{
        if (PlayerShadowInteraction.m_CurrentPlayerState != PlayerShadowInteraction.PLAYERSTATE.FORM || GameController.paused)
            return;

        if (other.gameObject.CompareTag("Player"))
		{
            pushCube.m_PlayerCanInteract = true;
            PlayerMotor.m_Instance.m_GrabbedObjectPlayerSide = gameObject.transform;
        }
	}

	void OnTriggerExit(Collider other)
	{
        if (GameController.paused)
            return;

        if (other.gameObject.CompareTag("Player"))
        {
            pushCube.m_PlayerCanInteract = false;
            PlayerMotor.m_Instance.m_GrabbedObjectPlayerSide = null;
            pushCube.Release();
        }
    }
}


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
        if (PlayerShadowInteraction.m_CurrentPlayerState != PlayerShadowInteraction.PlayerState.Form || GameController.m_Paused)
            return;

        if (other.gameObject.CompareTag("Player"))
		{
            pushCube.m_PlayerCanInteract = true;
            PlayerMotor.m_Instance.m_GrabbedObjectPlayerSide = gameObject.transform;
        }
	}

	void OnTriggerExit(Collider other)
	{
        if (GameController.m_Paused)
            return;

        if (other.gameObject.CompareTag("Player"))
        {
            pushCube.m_PlayerCanInteract = false;
            pushCube.Release();
        }
    }
}


using UnityEngine;

public class PropellorPlatform : MonoBehaviour {
	[Range(60, 120)] public float m_RotationSpeed;
    [SerializeField] bool m_RotateClockwise = true;

	void Update () 
	{
		if(PlayerShadowInteraction.m_CurrentPlayerState != PlayerShadowInteraction.PlayerState.Shifting && !GameController.m_Resetting && !GameController.m_Paused)
        {
            if(m_RotateClockwise)
            {
                transform.Rotate(Vector3.up, Time.deltaTime * m_RotationSpeed);
            }
            else
            {
                transform.Rotate(Vector3.up, Time.deltaTime * -m_RotationSpeed);
            }
        }
    }
}
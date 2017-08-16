using UnityEngine;

public class PlayerShadowSquishCheck : MonoBehaviour
{
    public bool m_Squished;

    void Update()
    {
        if(m_Squished)
        {
            GameController.m_Instance.ResetLevel();
            m_Squished = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (PlayerShadowInteraction.m_CurrentPlayerState == PlayerShadowInteraction.PlayerState.Shadow)
        {
            if (other.gameObject.tag != "Player" && !other.isTrigger)
            {
                m_Squished = true;
            }
        }
    }
}

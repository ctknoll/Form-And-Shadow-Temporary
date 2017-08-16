using UnityEngine;

public class PlayerSquishCheck : MonoBehaviour
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
        if (PlayerShadowInteraction.m_CurrentPlayerState == PlayerShadowInteraction.PlayerState.Form)
        {
            if (other.gameObject.tag != "Player" && !other.isTrigger)
            {
                m_Squished = true;
            }
        }
    }
}

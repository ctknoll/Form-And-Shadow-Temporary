using UnityEngine;

public class PlayerShadowSquishCheck : MonoBehaviour
{
    public bool squished2D;
    GameController gameController;

    void Awake()
    {
        
    }

    void Update()
    {
        if(squished2D)
        {
            Debug.Log("squished");
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (PlayerShadowInteraction.m_CurrentPlayerState == PlayerShadowInteraction.PlayerState.Shadow)
        {
            if (other.gameObject.tag != "Player" && !other.isTrigger)
            {
                squished2D = true;
            }
        }
    }
}

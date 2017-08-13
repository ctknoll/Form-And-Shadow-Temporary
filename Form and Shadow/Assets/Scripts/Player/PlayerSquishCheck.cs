using UnityEngine;

public class PlayerSquishCheck : MonoBehaviour
{
    public bool squished3D;
    GameController gameController;

    void Awake()
    {

    }

    void Update()
    {
        if(squished3D)
        {
            Debug.Log("squished");
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (PlayerShadowInteraction.m_CurrentPlayerState == PlayerShadowInteraction.PlayerState.Form)
        {
            if (other.gameObject.tag != "Player" && !other.isTrigger)
            {
                squished3D = true;
            }
        }
    }
}

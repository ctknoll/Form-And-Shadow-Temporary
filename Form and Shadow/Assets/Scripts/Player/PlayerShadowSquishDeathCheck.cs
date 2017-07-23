using UnityEngine;

public class PlayerShadowSquishDeathCheck : MonoBehaviour
{
    private GameController gameController;

    void Start()
    {
        gameController = GameObject.Find("Game_Controller").GetComponent<GameController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (NewPlayerShadowInteraction.m_CurrentPlayerState == NewPlayerShadowInteraction.PLAYERSTATE.SHADOW)
        {
            if (other.gameObject.tag != "Player" && !other.isTrigger)
            {
                if (!GameController.resetting)
                {
                    StartCoroutine(gameController.ResetLevel(false, false));
                }
            }
        }
    }
}

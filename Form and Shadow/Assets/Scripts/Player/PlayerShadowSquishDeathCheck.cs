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
        if (PlayerShadowInteraction.m_CurrentPlayerState == PlayerShadowInteraction.PlayerState.Shadow)
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

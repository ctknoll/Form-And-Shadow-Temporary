using UnityEngine;

public class PlayerGroundedZone2D : MonoBehaviour
{

    PlayerShadowInteraction playerMovement;

    void Start()
    {
        playerMovement = GameObject.Find("Player_Character").GetComponent<PlayerShadowInteraction>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!PlayerShadowInteraction.grounded2D && !PlayerShadowInteraction.shadowShiftingIn && !PlayerShadowInteraction.shadowShiftingOut)
        {
            if (other.GetComponent<Collider>().isTrigger != true && other.gameObject.tag != "Player")
            {
                playerMovement.playerAudioSource.clip = playerMovement.jumpLandClip;
                playerMovement.playerAudioSource.Play();
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Collider>().isTrigger != true && other.gameObject.tag != "Player")
            PlayerShadowInteraction.grounded2D = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Collider>().isTrigger != true && other.gameObject.tag != "Player")
            PlayerShadowInteraction.grounded2D = false;
    }
}


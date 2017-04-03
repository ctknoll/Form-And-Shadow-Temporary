using UnityEngine;

public class PlayerGroundedZone2D : MonoBehaviour{

    PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = GameObject.Find("Player_Character").GetComponent<PlayerMovement>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!PlayerMovement.grounded2D && !PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut)
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
		if(other.GetComponent<Collider>().isTrigger != true)
		    PlayerMovement.grounded2D = true;
	}

	void OnTriggerExit(Collider other)
	{
		if(other.GetComponent<Collider>().isTrigger !=true)
		    PlayerMovement.grounded2D = false;
	}
}


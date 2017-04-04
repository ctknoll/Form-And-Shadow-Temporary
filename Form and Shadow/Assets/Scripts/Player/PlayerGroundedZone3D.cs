using UnityEngine;

public class PlayerGroundedZone3D : MonoBehaviour{
    PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = GameObject.Find("Player_Character").GetComponent<PlayerMovement>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(!PlayerMovement.grounded3D && !PlayerMovement.shadowShiftingIn && !PlayerMovement.shadowShiftingOut)
        {
            if (other.GetComponent<Collider>().isTrigger != true && other.gameObject.layer != LayerMask.NameToLayer("Shadowmeld Ignore") && other.gameObject.tag != "Player")
            {
                playerMovement.playerAudioSource.clip = playerMovement.jumpLandClip;
                playerMovement.playerAudioSource.Play();
            }
        }
    }
	void OnTriggerStay(Collider other)
	{
		if(other.GetComponent<Collider>().isTrigger != true && other.gameObject.layer != LayerMask.NameToLayer("Shadowmeld Ignore"))
		    PlayerMovement.grounded3D = true;
	}

	void OnTriggerExit(Collider other)
	{
		if(other.GetComponent<Collider>().isTrigger != true && other.gameObject.layer != LayerMask.NameToLayer("Shadowmeld Ignore"))
		    PlayerMovement.grounded3D = false;
	}
}


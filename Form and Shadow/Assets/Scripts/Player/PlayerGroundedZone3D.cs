//using UnityEngine;

//public class PlayerGroundedZone3D : MonoBehaviour{
//    PlayerShadowInteraction playerMovement;

//    void Start()
//    {
//        playerMovement = GameObject.Find("Player_Character").GetComponent<PlayerShadowInteraction>();
//    }

//    void OnTriggerEnter(Collider other)
//    {
//        if(!PlayerShadowInteraction.grounded3D && !PlayerShadowInteraction.shadowShiftingIn && !PlayerShadowInteraction.shadowShiftingOut)
//        {
//            if (other.GetComponent<Collider>().isTrigger != true && other.gameObject.layer != LayerMask.NameToLayer("Shadowmeld Ignore") && other.gameObject.tag != "Player")
//            {
//                playerMovement.playerAudioSource.clip = playerMovement.jumpLandClip;
//                playerMovement.playerAudioSource.Play();
//            }
//        }
//    }
//	void OnTriggerStay(Collider other)
//	{
//		if(other.GetComponent<Collider>().isTrigger != true && other.gameObject.layer != LayerMask.NameToLayer("Shadowmeld Ignore") && other.gameObject.tag != "Player")
//		    PlayerShadowInteraction.grounded3D = true;
//	}

//	void OnTriggerExit(Collider other)
//	{
//		if(other.GetComponent<Collider>().isTrigger != true && other.gameObject.layer != LayerMask.NameToLayer("Shadowmeld Ignore") && other.gameObject.tag != "Player")
//		    PlayerShadowInteraction.grounded3D = false;
//	}
//}


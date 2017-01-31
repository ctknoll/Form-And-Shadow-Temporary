using UnityEngine;
using UnityEngine.SceneManagement;

public class Killzone : MonoBehaviour 
{
	public GameObject player;

	void Start()
	{
		player = GameObject.Find("Player_Character");
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			if(PlayerMovement.in3DSpace)
			{
				other.gameObject.transform.position = PlayerMovement.playerStartPosition;
			}
			else
			{
				PlayerMovement.in3DSpace = true;
				other.gameObject.GetComponent<CharacterController>().enabled = false;
				player.GetComponent<CharacterController>().enabled = true;
				player.GetComponent<PlayerMovement>().controller = player.GetComponent<CharacterController>();
				player.transform.position = PlayerMovement.playerStartPosition;
			}
		}
	}
}

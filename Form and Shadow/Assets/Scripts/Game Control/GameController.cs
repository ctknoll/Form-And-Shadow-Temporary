using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	public static bool resetting;
	public int score; //JSM 2.12.17 -- Score for Pickups

	private GameObject player;
	private GameObject playerShadow;

	void Start () 
	{
		player = GameObject.Find("Player_Character");
		playerShadow = GameObject.Find("Player_Shadow");
	}

	public IEnumerator ResetLevel()
	{
		resetting = true;
		yield return new WaitForSeconds(0.01f);

		// Reset the player's position
		if(!PlayerMovement.in3DSpace)
		{
			PlayerMovement.in3DSpace = true;
			playerShadow.GetComponent<CharacterController>().enabled = false;
			player.GetComponent<CharacterController>().enabled = true;
			player.GetComponent<PlayerMovement>().controller = player.GetComponent<CharacterController>();
		}
		player.transform.position = PlayerMovement.playerStartPosition;
		resetting = false;

	}

	//JSM 2.12.17 -- Simple script for flexible score addition
	public void scoreIncrement(int amount)
	{
		score += amount;
	}
}

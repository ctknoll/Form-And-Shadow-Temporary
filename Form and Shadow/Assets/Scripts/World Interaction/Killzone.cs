using UnityEngine;
using System.Collections;

public class Killzone : MonoBehaviour 
{
	private GameObject player;
	private GameController gameController;

	void Start()
	{
		player = GameObject.Find("Player_Character");
		gameController = GameObject.Find("Game_Controller").GetComponent<GameController>();
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			StartCoroutine(gameController.ResetLevel());
		}
	}
}

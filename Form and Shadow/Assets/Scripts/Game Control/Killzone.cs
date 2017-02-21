using UnityEngine;

public class Killzone : MonoBehaviour 
{
	private GameController gameController;

	void Start()
	{
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

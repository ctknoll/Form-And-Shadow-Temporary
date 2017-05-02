using UnityEngine;

public class Killzone : MonoBehaviour 
{
    public bool waterKillZone;
	private GameController gameController;

	void Start()
	{
		gameController = GameObject.Find("Game_Controller").GetComponent<GameController>();
	}

	void OnTriggerEnter(Collider other)
	{
        if (other.gameObject.tag == "Player")
		{
            if(!GameController.resetting)
            {
                if(waterKillZone)
                    StartCoroutine(gameController.ResetLevel(false, true));
                else
                    StartCoroutine(gameController.ResetLevel(false, false));
            }
		}
	}
}

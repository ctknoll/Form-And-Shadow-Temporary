using UnityEngine;

public class PlayerSquishDeathCheck : MonoBehaviour {
    private GameController gameController;

	void Start()
	{
		gameController = GameObject.Find("Game_Controller").GetComponent<GameController>();
	}

	void OnTriggerEnter(Collider other)
	{
        if(!PlayerMovement.shadowMelded)
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

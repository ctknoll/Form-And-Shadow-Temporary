using UnityEngine;

public class Pickup : MonoBehaviour 
{
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			GameObject.Find("Game_Controller").GetComponent<GameController>().ScoreIncrement(100);
            Destroy(gameObject);
		}
	}
}
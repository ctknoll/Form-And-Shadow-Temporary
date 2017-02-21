using UnityEngine;

public class Pickup : MonoBehaviour 
{
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			GameController.ScoreIncrement(100);
            Destroy(gameObject);
		}
	}
}
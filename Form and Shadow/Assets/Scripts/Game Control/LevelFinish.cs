using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFinish : MonoBehaviour 
{
	void OnTriggerEnter (Collider other) 
	{
		if(other.gameObject.tag == "Player")
		{
			SceneManager.LoadScene("Game_Over");
		}
	}
}

using UnityEngine;

public class LevelFinish : MonoBehaviour 
{
	void OnTriggerEnter (Collider other) 
	{
		Application.Quit();
	}
}
